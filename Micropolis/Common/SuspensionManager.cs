using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Micropolis.Common
{
    /// <summary>
    /// SuspensionManager erfasst den globalen Sitzungszustand, um die Verwaltung der Prozesslebensdauer
    /// für eine Anwendung zu vereinfachen.  Beachten, dass der Sitzungszustand bei einer Vielzahl von Bedingungen
    /// automatisch gelöscht wird und niemals zum Speichern von Informationen verwendet werden sollte, die zwischen Sitzungen zwar bequem übertragen werden können,
    /// jedoch beim Absturz der Anwendung gelöscht werden sollen oder
    /// aktualisiert werden.
    /// </summary>
    internal sealed class SuspensionManager
    {
        private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();
        private static List<Type> _knownTypes = new List<Type>();
        private const string sessionStateFilename = "_sessionState.xml";

        /// <summary>
        /// Bietet Zugriff auf den globalen Sitzungszustand für die aktuelle Sitzung.  Dieser Zustand wird
        /// von <see cref="SaveAsync"/> serialisiert und von
        /// <see cref="RestoreAsync"/> wiederhergestellt, sodass die Werte durch
        /// <see cref="DataContractSerializer"/> serialisierbar sein müssen und so kompakt wie möglich sein sollten.  Zeichenfolgen
        /// und andere eigenständige Datentypen werden dringend empfohlen.
        /// </summary>
        public static Dictionary<string, object> SessionState
        {
            get { return _sessionState; }
        }

        /// <summary>
        /// Liste mit benutzerdefinierten Typen, die für <see cref="DataContractSerializer"/> beim
        /// Lesen und Schreiben des Sitzungszustands bereitgestellt werden.  Diese ist zu Beginn leer, und zusätzliche Typen können zum
        /// Anpassen des Serialisierungsvorgangs hinzugefügt werden.
        /// </summary>
        public static List<Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        /// <summary>
        /// Den aktuellen <see cref="SessionState"/> speichern.  Alle <see cref="Frame"/>-Instanzen,
        /// die bei <see cref="RegisterFrame"/> registriert wurden, behalten ebenfalls ihren aktuellen
        /// Navigationsstapel bei, wodurch deren aktive <see cref="Page"/> eine Gelegenheit
        /// zum Speichern des zugehörigen Zustands erhält.
        /// </summary>
        /// <returns>Eine asynchrone Aufgabe, die das Speichern des Sitzungszustands wiedergibt.</returns>
        public static async Task SaveAsync()
        {
            try
            {
                // Navigationszustand für alle registrierten Rahmen speichern
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        SaveFrameNavigationState(frame);
                    }
                }

                // Sitzungszustand synchron serialisieren, um einen asynchronen Zugriff auf den freigegebenen
                // Zustand
                MemoryStream sessionData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                serializer.WriteObject(sessionData, _sessionState);

                // Einen Ausgabedatenstrom für die SessionState-Datei abrufen und den Zustand asynchron schreiben
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(sessionStateFilename, CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        /// <summary>
        /// Stellt den zuvor gespeicherten <see cref="SessionState"/> wieder her.  Für alle <see cref="Frame"/>-Instanzen,
        /// die bei <see cref="RegisterFrame"/> registriert wurden, wird ebenfalls der vorherige
        /// Navigationszustand wiederhergestellt, wodurch deren aktive <see cref="Page"/> eine Gelegenheit zum Wiederherstellen
        /// des Zustands erhält.
        /// </summary>
        /// <param name="sessionBaseKey">Ein optionaler Schlüssel zum Identifizieren des Typs der Sitzung.
        /// Damit können verschiedene Szenarien für den Anwendungsstart unterschieden werden.</param>
        /// <returns>Eine asynchrone Aufgabe, die das Lesen des Sitzungszustands wiedergibt.  Auf den
        /// Inhalt von <see cref="SessionState"/> sollte erst zurückgegriffen werden, wenn diese Aufgabe
        /// abgeschlossen ist.</returns>
        public static async Task RestoreAsync(String sessionBaseKey = null)
        {
            _sessionState = new Dictionary<String, Object>();

            try
            {
                // Eingabedatenstrom für die SessionState-Datei abrufen
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    // Sitzungszustand deserialisieren
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                    _sessionState = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
                }

                // Alle registrierten Rahmen auf den gespeicherten Zustand wiederherstellen
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame) && (string)frame.GetValue(FrameSessionBaseKeyProperty) == sessionBaseKey)
                    {
                        frame.ClearValue(FrameSessionStateProperty);
                        RestoreFrameNavigationState(frame);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        private static DependencyProperty FrameSessionStateKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(String), typeof(SuspensionManager), null);
        private static DependencyProperty FrameSessionBaseKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionBaseKeyParams", typeof(String), typeof(SuspensionManager), null);
        private static DependencyProperty FrameSessionStateProperty =
            DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<String, Object>), typeof(SuspensionManager), null);
        private static List<WeakReference<Frame>> _registeredFrames = new List<WeakReference<Frame>>();

        /// <summary>
        /// Registriert eine <see cref="Frame"/>-Instanz, um den zugehörigen Navigationsverlauf mithilfe von
        /// <see cref="SessionState"/> speichern und wiederherstellen zu können.  Rahmen sollten direkt nach der Erstellung
        /// registriert werden, wenn diese Bestandteil der Verwaltung des Sitzungszustands sind.  Wenn der
        /// Zustand für den speziellen Schlüssel bereits wiederhergestellt wurde,
        /// wird der Navigationsverlauf bei der Registrierung sofort wiederhergestellt.  Bei nachfolgenden Aufrufen von
        /// <see cref="RestoreAsync"/> wird der Navigationsverlauf ebenfalls wiederhergestellt.
        /// </summary>
        /// <param name="frame">Eine Instanz, deren Navigationsverlauf von
        /// <see cref="SuspensionManager"/></param>
        /// <param name="sessionStateKey">Ein eindeutiger Schlüssel in <see cref="SessionState"/> zum
        /// Speichern von navigationsbezogenen Informationen.</param>
        /// <param name="sessionBaseKey">Ein optionaler Schlüssel zum Identifizieren des Typs der Sitzung.
        /// Damit können verschiedene Szenarien für den Anwendungsstart unterschieden werden.</param>
        public static void RegisterFrame(Frame frame, String sessionStateKey, String sessionBaseKey = null)
        {
            if (frame.GetValue(FrameSessionStateKeyProperty) != null)
            {
                throw new InvalidOperationException("Frames can only be registered to one session state key");
            }

            if (frame.GetValue(FrameSessionStateProperty) != null)
            {
                throw new InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all");
            }

            if (!string.IsNullOrEmpty(sessionBaseKey))
            {
                frame.SetValue(FrameSessionBaseKeyProperty, sessionBaseKey);
                sessionStateKey = sessionBaseKey + "_" + sessionStateKey;
            }

            // Eine Abhängigkeitseigenschaft verwenden, um den Sitzungsschlüssel mit einem Rahmen zu verknüpfen, und eine Liste von Rahmen speichern, deren
            // Navigationszustand verwaltet werden soll
            frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
            _registeredFrames.Add(new WeakReference<Frame>(frame));

            // Überprüfen, ob der Navigationszustand wiederhergestellt werden kann
            RestoreFrameNavigationState(frame);
        }

        /// <summary>
        /// Hebt die Verknüpfung eines <see cref="Frame"/>, der zuvor durch <see cref="RegisterFrame"/> registriert wurde,
        /// mit <see cref="SessionState"/> auf.  Alle zuvor erfassten Navigationszustände werden
        /// entfernt.
        /// </summary>
        /// <param name="frame">Eine Instanz, deren Navigationsverlauf nicht mehr
        /// verwaltet werden soll.</param>
        public static void UnregisterFrame(Frame frame)
        {
            // Sitzungszustand und Rahmen aus der Liste der Rahmen entfernen, deren Navigationszustand
            // gespeichert wird (gemeinsam mit allen schwachen Verweisen, die nicht mehr erreichbar sind)
            SessionState.Remove((String)frame.GetValue(FrameSessionStateKeyProperty));
            _registeredFrames.RemoveAll((weakFrameReference) =>
            {
                Frame testFrame;
                return !weakFrameReference.TryGetTarget(out testFrame) || testFrame == frame;
            });
        }

        /// <summary>
        /// Bietet Speichermöglichkeit für den Sitzungszustand, der mit dem angegebenen <see cref="Frame"/> verknüpft ist.
        /// Für Rahmen, die zuvor mit <see cref="RegisterFrame"/> registriert wurden, wird der
        /// Sitzungszustand automatisch als Teil des globalen <see cref="SessionState"/>
        /// gespeichert und wiederhergestellt.  Rahmen, die nicht registriert sind, verfügen über einen vorübergehenden Zustand,
        /// der weiterhin nützlich sein kann, wenn Seiten wiederhergestellt werden, die aus dem
        /// im Navigationscache verworfen wird.
        /// </summary>
        /// <remarks>Apps können beim Verwalten des seitenspezifischen Zustands auf <see cref="NavigationHelper"/> zurückgreifen,
        /// anstatt direkt mit dem Rahmensitzungszustand zu arbeiten.</remarks>
        /// <param name="frame">Die Instanz, für die der Sitzungszustand gewünscht wird.</param>
        /// <returns>Eine Auflistung des Zustands, für den der gleiche Serialisierungsmechanismus wie für
        /// <see cref="SessionState"/>.</returns>
        public static Dictionary<String, Object> SessionStateForFrame(Frame frame)
        {
            var frameState = (Dictionary<String, Object>)frame.GetValue(FrameSessionStateProperty);

            if (frameState == null)
            {
                var frameSessionKey = (String)frame.GetValue(FrameSessionStateKeyProperty);
                if (frameSessionKey != null)
                {
                    // Registrierte Rahmen geben den entsprechenden Sitzungszustand wieder.
                    if (!_sessionState.ContainsKey(frameSessionKey))
                    {
                        _sessionState[frameSessionKey] = new Dictionary<String, Object>();
                    }
                    frameState = (Dictionary<String, Object>)_sessionState[frameSessionKey];
                }
                else
                {
                    // Rahmen, die nicht registriert sind, verfügen über einen vorübergehenden Zustand
                    frameState = new Dictionary<String, Object>();
                }
                frame.SetValue(FrameSessionStateProperty, frameState);
            }
            return frameState;
        }

        private static void RestoreFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            if (frameState.ContainsKey("Navigation"))
            {
                frame.SetNavigationState((String)frameState["Navigation"]);
            }
        }

        private static void SaveFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            frameState["Navigation"] = frame.GetNavigationState();
        }
    }
    public class SuspensionManagerException : Exception
    {
        public SuspensionManagerException()
        {
        }

        public SuspensionManagerException(Exception e)
            : base("SuspensionManager failed", e)
        {

        }
    }
}
