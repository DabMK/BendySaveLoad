using System;
using System.Runtime.InteropServices;

namespace BendySaveLoad
{
    internal class Checker
    {
        private static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static Guid localLowId = new("A520A1A4-1780-4FF6-BD18-167343C5AF16");
        public static string gameFiles = @$"{appData[..appData.LastIndexOf('\\')]}\LocalLow\Joey Drew Studios\Bendy and the Ink Machine\Saves";
        public static bool IsGameInstalled()
        {
            if (Directory.Exists(gameFiles))
            {
                return true;
            }
            else
            {
                string tmpGameFiles = $@"{GetKnownFolderPath(localLowId)}\Joey Drew Studios\Bendy and the Ink Machine\Saves";
                if (Directory.Exists(tmpGameFiles))
                {
                    gameFiles = tmpGameFiles;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool WasGamePlayed()
        {
            return File.Exists(@$"{gameFiles}\batim.game");
        }

        private static string GetKnownFolderPath(Guid knownFolderId)
        {
            IntPtr pszPath = IntPtr.Zero;
            try
            {
                int hr = SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero, out pszPath);
                if (hr >= 0) { return Marshal.PtrToStringAuto(pszPath); }
                throw Marshal.GetExceptionForHR(hr);
            }
            finally
            {
                if (pszPath != IntPtr.Zero) { Marshal.FreeCoTaskMem(pszPath); }
            }
        }
        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    }
}