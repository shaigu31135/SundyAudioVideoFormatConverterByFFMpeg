using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FFMPEG快速格式转换 {
    public static class RegistryOperation {
        [DllImport ( "shell32.dll", EntryPoint = "ShellExecuteA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = false )] public static extern long ShellExecute ( IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd );

        public static bool DelReg ( RegistryKey rootKey, string Node ) {
            bool result = false;
            try {
                RegistryKey rKey = rootKey.OpenSubKey ( Node );
                rootKey.DeleteSubKey ( Node + @"\command", true );
                rootKey.DeleteSubKey ( Node, true );
                rootKey.Close ();
                result = true;
            } catch ( Exception ex ) {
                MessageBox.Show ( ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
                result = false;
            }
            return result;
        }

        public static bool AddReg ( RegistryKey rootKey, string Node, string MenuName ) {
            bool result = false;
            try {
                RegistryKey rKey = rootKey.OpenSubKey ( Node );
                rKey = rootKey.CreateSubKey ( Node );
                rKey.SetValue ( name: "Icon", value: Application.ExecutablePath, valueKind: RegistryValueKind.ExpandString );
                rKey.SetValue ( name: "MUIVerb", value: (object) MenuName, valueKind: RegistryValueKind.String );
                RegistryKey CmdKey = rKey.CreateSubKey ( "command" );
                CmdKey.SetValue ( name: "", value: "\"" + Application.ExecutablePath + "\" \"%1\"", valueKind: RegistryValueKind.String );
                CmdKey.Close ();
                rKey.Close ();
                rootKey.Close ();
                result = true;
            } catch ( Exception ex ) {
                MessageBox.Show ( ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
                result = false;
            }
            return result;
        }

        public static bool isRegExist ( RegistryKey rootKey, string Node ) {
            try {
                RegistryKey rKey = rootKey.OpenSubKey ( Node );
                if ( rKey != null ) {
                    rKey.Close ();
                    rootKey.Close ();
                    return true;
                } else {
                    return false;
                }
            } catch ( Exception ex ) {
                Console.WriteLine ( ex.Message );
                return false;
            }
        }
    }
}
