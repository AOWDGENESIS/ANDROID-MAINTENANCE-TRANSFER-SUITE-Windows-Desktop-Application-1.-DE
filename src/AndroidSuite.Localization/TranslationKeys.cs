namespace AndroidSuite.Localization;

/// <summary>
/// Stabile Schluesselnamen aller Oberflaechentexte (§2: kein hartcodierter Text).
/// Die Konstanten verhindern Tippfehler; ein Test prueft, dass jede Konstante
/// in jedem Sprachkatalog vorhanden ist.
/// </summary>
public static class TranslationKeys
{
    // --- Anwendung / Application ---
    /// <summary>DE: Android Wartungs- und Übertragungs-Suite</summary>
    public const string AppTitle = "app.title";
    /// <summary>DE: Android Suite</summary>
    public const string AppShortTitle = "app.shortTitle";
    /// <summary>DE: Version {0}</summary>
    public const string AppVersion = "app.version";
    /// <summary>DE: Offline-First: Ihre Daten verlassen diesen PC nicht.</summary>
    public const string AppOfflineFirst = "app.offlineFirst";

    // --- Allgemein / Common ---
    /// <summary>DE: OK</summary>
    public const string CommonOk = "common.ok";
    /// <summary>DE: Abbrechen</summary>
    public const string CommonCancel = "common.cancel";
    /// <summary>DE: Ja</summary>
    public const string CommonYes = "common.yes";
    /// <summary>DE: Nein</summary>
    public const string CommonNo = "common.no";
    /// <summary>DE: Schließen</summary>
    public const string CommonClose = "common.close";
    /// <summary>DE: Zurück</summary>
    public const string CommonBack = "common.back";
    /// <summary>DE: Weiter</summary>
    public const string CommonNext = "common.next";
    /// <summary>DE: Wiederholen</summary>
    public const string CommonRetry = "common.retry";
    /// <summary>DE: Speichern</summary>
    public const string CommonSave = "common.save";
    /// <summary>DE: Übernehmen</summary>
    public const string CommonApply = "common.apply";
    /// <summary>DE: Öffnen</summary>
    public const string CommonOpen = "common.open";
    /// <summary>DE: Löschen</summary>
    public const string CommonDelete = "common.delete";
    /// <summary>DE: Verschieben</summary>
    public const string CommonMove = "common.move";
    /// <summary>DE: Kopieren</summary>
    public const string CommonCopy = "common.copy";
    /// <summary>DE: Umbenennen</summary>
    public const string CommonRename = "common.rename";
    /// <summary>DE: Aktualisieren</summary>
    public const string CommonRefresh = "common.refresh";
    /// <summary>DE: Suchen</summary>
    public const string CommonSearch = "common.search";
    /// <summary>DE: Alle auswählen</summary>
    public const string CommonSelectAll = "common.selectAll";
    /// <summary>DE: Auswahl aufheben</summary>
    public const string CommonDeselectAll = "common.deselectAll";
    /// <summary>DE: Details</summary>
    public const string CommonDetails = "common.details";
    /// <summary>DE: Eigenschaften</summary>
    public const string CommonProperties = "common.properties";
    /// <summary>DE: Warnung</summary>
    public const string CommonWarning = "common.warning";
    /// <summary>DE: Information</summary>
    public const string CommonInformation = "common.information";
    /// <summary>DE: Unbekannt</summary>
    public const string CommonUnknown = "common.unknown";
    /// <summary>DE: Nicht verfügbar</summary>
    public const string CommonNotAvailable = "common.notAvailable";
    /// <summary>DE: Wird geladen …</summary>
    public const string CommonLoading = "common.loading";
    /// <summary>DE: {0} Dateien</summary>
    public const string CommonFilesCount = "common.filesCount";
    /// <summary>DE: {0} Bytes</summary>
    public const string CommonSizeBytes = "common.sizeBytes";
    /// <summary>DE: {0} von {1}</summary>
    public const string CommonOf = "common.of";

    // --- Hauptnavigation (§43) / Main navigation (§43) ---
    /// <summary>DE: Übersicht</summary>
    public const string NavDashboard = "nav.dashboard";
    /// <summary>DE: Gerät</summary>
    public const string NavDevice = "nav.device";
    /// <summary>DE: Dateimanager</summary>
    public const string NavFileManager = "nav.fileManager";
    /// <summary>DE: Übertragung</summary>
    public const string NavTransfer = "nav.transfer";
    /// <summary>DE: Sicherung</summary>
    public const string NavBackup = "nav.backup";
    /// <summary>DE: Wiederherstellung</summary>
    public const string NavRestore = "nav.restore";
    /// <summary>DE: Speicher</summary>
    public const string NavStorage = "nav.storage";
    /// <summary>DE: Cleanup Center</summary>
    public const string NavCleanup = "nav.cleanup";
    /// <summary>DE: Medien</summary>
    public const string NavMedia = "nav.media";
    /// <summary>DE: Apps</summary>
    public const string NavApps = "nav.apps";
    /// <summary>DE: Sicherheit</summary>
    public const string NavSecurity = "nav.security";
    /// <summary>DE: Live-Bildschirm</summary>
    public const string NavLiveScreen = "nav.liveScreen";
    /// <summary>DE: Diagnose</summary>
    public const string NavDiagnostics = "nav.diagnostics";
    /// <summary>DE: Protokolle</summary>
    public const string NavLogs = "nav.logs";
    /// <summary>DE: Einstellungen</summary>
    public const string NavSettings = "nav.settings";
    /// <summary>DE: Später verfügbar</summary>
    public const string NavAvailableLater = "nav.availableLater";

    // --- Gerät (§7, §44) / Device (§7, §44) ---
    /// <summary>DE: Gerät verbunden</summary>
    public const string DeviceConnected = "device.connected";
    /// <summary>DE: Gerät getrennt</summary>
    public const string DeviceDisconnected = "device.disconnected";
    /// <summary>DE: Kein Gerät gefunden</summary>
    public const string DeviceNone = "device.none";
    /// <summary>DE: Gerät wird gesucht …</summary>
    public const string DeviceSearching = "device.searching";
    /// <summary>DE: Hersteller</summary>
    public const string DeviceManufacturer = "device.manufacturer";
    /// <summary>DE: Modell</summary>
    public const string DeviceModel = "device.model";
    /// <summary>DE: Android-Version</summary>
    public const string DeviceAndroidVersion = "device.androidVersion";
    /// <summary>DE: API-Level</summary>
    public const string DeviceApiLevel = "device.apiLevel";
    /// <summary>DE: Seriennummer</summary>
    public const string DeviceSerial = "device.serial";
    /// <summary>DE: Batterie</summary>
    public const string DeviceBattery = "device.battery";
    /// <summary>DE: Interner Speicher</summary>
    public const string DeviceInternalStorage = "device.internalStorage";
    /// <summary>DE: SD-Karte</summary>
    public const string DeviceSdCard = "device.sdCard";
    /// <summary>DE: Keine SD-Karte erkannt</summary>
    public const string DeviceSdCardNotPresent = "device.sdCardNotPresent";
    /// <summary>DE: Letzte Verbindung</summary>
    public const string DeviceLastConnection = "device.lastConnection";
    /// <summary>DE: ADB-Status</summary>
    public const string DeviceAdbStatus = "device.adbStatus";
    /// <summary>DE: MTP-Status</summary>
    public const string DeviceMtpStatus = "device.mtpStatus";
    /// <summary>DE: Autorisiert</summary>
    public const string DeviceAdbAuthorized = "device.adb.authorized";
    /// <summary>DE: Nicht autorisiert</summary>
    public const string DeviceAdbUnauthorized = "device.adb.unauthorized";
    /// <summary>DE: Offline</summary>
    public const string DeviceAdbOffline = "device.adb.offline";
    /// <summary>DE: ADB nicht verfügbar</summary>
    public const string DeviceAdbUnavailable = "device.adb.unavailable";
    /// <summary>DE: Gesperrt</summary>
    public const string DeviceStateLocked = "device.state.locked";
    /// <summary>DE: Entsperrt</summary>
    public const string DeviceStateUnlocked = "device.state.unlocked";

    // --- Übersicht (§44) / Dashboard (§44) ---
    /// <summary>DE: Übersicht</summary>
    public const string DashboardTitle = "dashboard.title";
    /// <summary>DE: Schnellaktionen</summary>
    public const string DashboardQuickActions = "dashboard.quickActions";
    /// <summary>DE: Gerät analysieren</summary>
    public const string DashboardAnalyzeDevice = "dashboard.analyzeDevice";
    /// <summary>DE: Dateien übertragen</summary>
    public const string DashboardTransferFiles = "dashboard.transferFiles";
    /// <summary>DE: Sicherung erstellen</summary>
    public const string DashboardCreateBackup = "dashboard.createBackup";
    /// <summary>DE: Speicher analysieren</summary>
    public const string DashboardAnalyzeStorage = "dashboard.analyzeStorage";
    /// <summary>DE: Cleanup Center öffnen</summary>
    public const string DashboardOpenCleanup = "dashboard.openCleanup";
    /// <summary>DE: Gesundheit</summary>
    public const string DashboardHealth = "dashboard.health";

    // --- Speicheranalyse (§39) / Storage analysis (§39) ---
    /// <summary>DE: Gesamtspeicher</summary>
    public const string StorageTotal = "storage.total";
    /// <summary>DE: Belegt</summary>
    public const string StorageUsed = "storage.used";
    /// <summary>DE: Frei</summary>
    public const string StorageFree = "storage.free";
    /// <summary>DE: System</summary>
    public const string StorageSystem = "storage.system";
    /// <summary>DE: Apps</summary>
    public const string StorageApps = "storage.apps";
    /// <summary>DE: {0} frei von {1}</summary>
    public const string StorageFreeOfTotal = "storage.freeOfTotal";
    /// <summary>DE: Was verbraucht meinen Speicher?</summary>
    public const string StorageWhatUsesSpace = "storage.whatUsesSpace";
    /// <summary>DE: Was könnte ich wahrscheinlich sicher freigeben?</summary>
    public const string StorageWhatCanBeFreed = "storage.whatCanBeFreed";
    /// <summary>DE: Vorher / Nachher</summary>
    public const string StorageBeforeAfter = "storage.beforeAfter";
    /// <summary>DE: Freigegebener Speicher: {0}</summary>
    public const string StorageFreedSpace = "storage.freedSpace";

    // --- Medienkategorien (§11) / Media categories (§11) ---
    /// <summary>DE: Bilder</summary>
    public const string MediaCategoryImage = "media.category.image";
    /// <summary>DE: Videos</summary>
    public const string MediaCategoryVideo = "media.category.video";
    /// <summary>DE: Animationen / GIF</summary>
    public const string MediaCategoryAnimation = "media.category.animation";
    /// <summary>DE: Musik</summary>
    public const string MediaCategoryMusic = "media.category.music";
    /// <summary>DE: Podcasts</summary>
    public const string MediaCategoryPodcast = "media.category.podcast";
    /// <summary>DE: Hörbücher</summary>
    public const string MediaCategoryAudiobook = "media.category.audiobook";
    /// <summary>DE: Sprachaufnahmen</summary>
    public const string MediaCategoryVoiceRecording = "media.category.voiceRecording";
    /// <summary>DE: Sprachnachrichten</summary>
    public const string MediaCategoryVoiceMessage = "media.category.voiceMessage";
    /// <summary>DE: WhatsApp-Medien</summary>
    public const string MediaCategoryWhatsapp = "media.category.whatsapp";
    /// <summary>DE: Telegram-Medien</summary>
    public const string MediaCategoryTelegram = "media.category.telegram";
    /// <summary>DE: Signal-Medien</summary>
    public const string MediaCategorySignal = "media.category.signal";
    /// <summary>DE: Downloads</summary>
    public const string MediaCategoryDownload = "media.category.download";
    /// <summary>DE: Dokumente</summary>
    public const string MediaCategoryDocument = "media.category.document";
    /// <summary>DE: APK</summary>
    public const string MediaCategoryApk = "media.category.apk";
    /// <summary>DE: Archive</summary>
    public const string MediaCategoryArchive = "media.category.archive";
    /// <summary>DE: Cache</summary>
    public const string MediaCategoryCache = "media.category.cache";
    /// <summary>DE: Sonstiges</summary>
    public const string MediaCategoryOther = "media.category.other";
    /// <summary>DE: Animiert</summary>
    public const string MediaAnimated = "media.animated";
    /// <summary>DE: Statisch</summary>
    public const string MediaStatic = "media.static";
    /// <summary>DE: Herkunft</summary>
    public const string MediaOrigin = "media.origin";
    /// <summary>DE: Zuverlässigkeit der Einstufung: {0} %</summary>
    public const string MediaConfidence = "media.confidence";
    /// <summary>DE: Grundlage der Einstufung</summary>
    public const string MediaEvidence = "media.evidence";

    // --- Cleanup Center (§23-§26) / Cleanup center (§23-§26) ---
    /// <summary>DE: Cleanup Center</summary>
    public const string CleanupTitle = "cleanup.title";
    /// <summary>DE: Große Dateien</summary>
    public const string CleanupLargeFiles = "cleanup.largeFiles";
    /// <summary>DE: Alte Dateien</summary>
    public const string CleanupOldFiles = "cleanup.oldFiles";
    /// <summary>DE: Duplikate</summary>
    public const string CleanupDuplicates = "cleanup.duplicates";
    /// <summary>DE: Analysieren</summary>
    public const string CleanupAnalyse = "cleanup.analyse";
    /// <summary>DE: Testlauf (nichts wird verändert)</summary>
    public const string CleanupDryRun = "cleanup.dryRun";
    /// <summary>DE: Vorschau</summary>
    public const string CleanupPreview = "cleanup.preview";
    /// <summary>DE: Bestätigen</summary>
    public const string CleanupConfirm = "cleanup.confirm";
    /// <summary>DE: Ausführen</summary>
    public const string CleanupExecute = "cleanup.execute";
    /// <summary>DE: Überprüfen</summary>
    public const string CleanupVerify = "cleanup.verify";
    /// <summary>DE: Bericht</summary>
    public const string CleanupReport = "cleanup.report";
    /// <summary>DE: Potenziell freigebbar: {0}</summary>
    public const string CleanupPotentiallyFreeable = "cleanup.potentiallyFreeable";
    /// <summary>DE: Geschützt</summary>
    public const string CleanupProtectedItem = "cleanup.protectedItem";
    /// <summary>DE: Prüfung erforderlich</summary>
    public const string CleanupNeedsReview = "cleanup.needsReview";
    /// <summary>DE: Verschieben statt Löschen</summary>
    public const string CleanupMoveInsteadOfDelete = "cleanup.moveInsteadOfDelete";
    /// <summary>DE: Vor dem Löschen sichern</summary>
    public const string CleanupBackupBeforeDelete = "cleanup.backupBeforeDelete";
    /// <summary>DE: Es wird nichts gelöscht, bevor Sie es ausdrücklich bestätigt haben.</summary>
    public const string CleanupNothingWithoutConfirmation = "cleanup.nothingWithoutConfirmation";
    /// <summary>DE: {0} Dateien ausgewählt, {1} insgesamt</summary>
    public const string CleanupSelectedSummary = "cleanup.selectedSummary";
    /// <summary>DE: {0} geschützte Elemente wurden nicht ausgewählt.</summary>
    public const string CleanupSkippedProtected = "cleanup.skippedProtected";

    // --- Übertragung (§19, §20) / Transfer (§19, §20) ---
    /// <summary>DE: Wartet</summary>
    public const string TransferStatusWaiting = "transfer.status.waiting";
    /// <summary>DE: Läuft</summary>
    public const string TransferStatusRunning = "transfer.status.running";
    /// <summary>DE: Angehalten</summary>
    public const string TransferStatusPaused = "transfer.status.paused";
    /// <summary>DE: Abgeschlossen</summary>
    public const string TransferStatusCompleted = "transfer.status.completed";
    /// <summary>DE: Fehlgeschlagen</summary>
    public const string TransferStatusFailed = "transfer.status.failed";
    /// <summary>DE: Abgebrochen</summary>
    public const string TransferStatusCancelled = "transfer.status.cancelled";
    /// <summary>DE: {0} von {1} Dateien</summary>
    public const string TransferProgressFiles = "transfer.progressFiles";
    /// <summary>DE: {0}/s</summary>
    public const string TransferSpeed = "transfer.speed";
    /// <summary>DE: Verbleibend: {0}</summary>
    public const string TransferEta = "transfer.eta";
    /// <summary>DE: Anhalten</summary>
    public const string TransferPause = "transfer.pause";
    /// <summary>DE: Fortsetzen</summary>
    public const string TransferResume = "transfer.resume";
    /// <summary>DE: Übertragung überprüft</summary>
    public const string TransferVerified = "transfer.verified";
    /// <summary>DE: Überprüfung fehlgeschlagen – die Datei gilt als nicht übertragen.</summary>
    public const string TransferVerificationFailed = "transfer.verificationFailed";
    /// <summary>DE: Fortsetzen erfolgt auf Dateiebene: angefangene Dateien werden erneut übertragen.</summary>
    public const string TransferResumeFileLevel = "transfer.resumeFileLevel";

    // --- Sicherheitshinweise (§4, §5, §27) / Safety notices (§4, §5, §27) ---
    /// <summary>DE: Sicherheitshinweis</summary>
    public const string SafetyTitle = "safety.title";
    /// <summary>DE: Eine Datei wird niemals allein wegen ihrer Dateiendung einer Kategorie zugeordnet oder gelöscht.</summary>
    public const string SafetyExtensionIsNotEnough = "safety.extensionIsNotEnough";
    /// <summary>DE: Musikdateien werden getrennt von Messenger-Audio behandelt und nie automatisch mitgelöscht.</summary>
    public const string SafetyMusicIsNotMessengerAudio = "safety.musicIsNotMessengerAudio";
    /// <summary>DE: Auf Flash-Speicher ist kein garantiertes physisches Überschreiben möglich. Gelöschte Dateien sind durch diese Anwendung nicht wiederherstellbar, eine forensische Wiederherstellung lässt sich jedoch nicht ausschließen.</summary>
    public const string SafetyFlashEraseLimitation = "safety.flashEraseLimitation";
    /// <summary>DE: Alles anhalten</summary>
    public const string SafetyStopAll = "safety.stopAll";
    /// <summary>DE: Alle laufenden Vorgänge sicher anhalten?</summary>
    public const string SafetyStopAllConfirm = "safety.stopAllConfirm";
    /// <summary>DE: Geschützte Ordner</summary>
    public const string SafetyProtectedFolders = "safety.protectedFolders";

    // --- Fehlermeldungen (§75) / Error messages (§75) ---
    /// <summary>DE: Fehler</summary>
    public const string ErrorTitle = "error.title";
    /// <summary>DE: Was ist passiert?</summary>
    public const string ErrorWhatHappened = "error.whatHappened";
    /// <summary>DE: Was bedeutet das?</summary>
    public const string ErrorWhatItMeans = "error.whatItMeans";
    /// <summary>DE: Was können Sie tun?</summary>
    public const string ErrorWhatToDo = "error.whatToDo";
    /// <summary>DE: Technische Details</summary>
    public const string ErrorTechnicalDetails = "error.technicalDetails";
    /// <summary>DE: Fehlercode: {0}</summary>
    public const string ErrorCode = "error.code";
    /// <summary>DE: Das Gerät hat diesen Computer nicht für USB-Debugging freigegeben.</summary>
    public const string ErrorAdbUnauthorizedWhat = "error.adbUnauthorized.what";
    /// <summary>DE: Android schützt Ihre Daten und verlangt eine ausdrückliche Bestätigung auf dem Gerät.</summary>
    public const string ErrorAdbUnauthorizedMeans = "error.adbUnauthorized.means";
    /// <summary>DE: Entsperren Sie das Gerät und bestätigen Sie die Abfrage „USB-Debugging zulassen“.</summary>
    public const string ErrorAdbUnauthorizedTodo = "error.adbUnauthorized.todo";
    /// <summary>DE: Auf den Ordner „Android/data“ kann nicht zugegriffen werden.</summary>
    public const string ErrorAndroidDataRestrictedWhat = "error.androidDataRestricted.what";
    /// <summary>DE: Seit Android 11 sperrt das Betriebssystem diesen Bereich. Das ist kein Fehler dieser Anwendung.</summary>
    public const string ErrorAndroidDataRestrictedMeans = "error.androidDataRestricted.means";
    /// <summary>DE: Verwenden Sie die Freigabefunktion der jeweiligen App oder sichern Sie die zugänglichen Medienordner.</summary>
    public const string ErrorAndroidDataRestrictedTodo = "error.androidDataRestricted.todo";
    /// <summary>DE: Auf dem Ziel ist nicht genügend Speicher frei.</summary>
    public const string ErrorTargetFullWhat = "error.targetFull.what";
    /// <summary>DE: Die Übertragung wurde angehalten, damit keine unvollständigen Dateien entstehen.</summary>
    public const string ErrorTargetFullMeans = "error.targetFull.means";
    /// <summary>DE: Geben Sie Speicher frei oder wählen Sie ein anderes Ziel.</summary>
    public const string ErrorTargetFullTodo = "error.targetFull.todo";

    // --- Einstellungen (§2, §46) / Settings (§2, §46) ---
    /// <summary>DE: Einstellungen</summary>
    public const string SettingsTitle = "settings.title";
    /// <summary>DE: Sprache</summary>
    public const string SettingsLanguage = "settings.language";
    /// <summary>DE: Deutsch</summary>
    public const string SettingsLanguageDe = "settings.language.de";
    /// <summary>DE: English</summary>
    public const string SettingsLanguageEn = "settings.language.en";
    /// <summary>DE: Die Sprache wird sofort umgestellt und dauerhaft gespeichert.</summary>
    public const string SettingsLanguageHint = "settings.languageHint";
    /// <summary>DE: Darstellung</summary>
    public const string SettingsAppearance = "settings.appearance";
    /// <summary>DE: Dunkles Design</summary>
    public const string SettingsDarkMode = "settings.darkMode";
    /// <summary>DE: ADB-Pfad</summary>
    public const string SettingsAdbPath = "settings.adbPath";
    /// <summary>DE: Parallele Übertragungen</summary>
    public const string SettingsParallelTransfers = "settings.parallelTransfers";
    /// <summary>DE: Protokollordner</summary>
    public const string SettingsLogFolder = "settings.logFolder";
    /// <summary>DE: Einstellungen gespeichert</summary>
    public const string SettingsSaved = "settings.saved";
    /// <summary>DE: Einstellungen konnten nicht gespeichert werden.</summary>
    public const string SettingsSaveFailed = "settings.saveFailed";
}
