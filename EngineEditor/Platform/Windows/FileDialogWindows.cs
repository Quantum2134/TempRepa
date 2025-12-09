using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.Platform.Windows
{
    public static class FileDialogWindows
    {
        /// <summary>
        /// Открыть один файл.
        /// </summary>
        public static string? OpenFile(string title = "Open File", string filter = "All Files (*.*)\0*.*\0")
        {
            var dialog = (IFileOpenDialog)new FileOpenDialog();
            try
            {
                var options = FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
                              FILEOPENDIALOGOPTIONS.FOS_NOVALIDATE |
                              FILEOPENDIALOGOPTIONS.FOS_FILEMUSTEXIST;
                dialog.SetOptions(options);

                if (!string.IsNullOrEmpty(title))
                    dialog.SetTitle(title);

                if (!string.IsNullOrEmpty(filter))
                {
                    var specs = MakeFilterSpec(filter);
                    dialog.SetFileTypes((uint)specs.Length, specs);
                }

                int hr = dialog.Show(IntPtr.Zero);
                if (hr != 0) return null; // отмена

                hr = dialog.GetResult(out IShellItem item);
                if (hr != 0) return null;

                try
                {
                    item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string path);
                    return path;
                }
                finally
                {
                    Marshal.ReleaseComObject(item);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(dialog);
            }
        }

        /// <summary>
        /// Открыть несколько файлов (только из одной папки).
        /// </summary>
        public static string[] OpenFiles(string title = "Open Files", string filter = "All Files (*.*)\0*.*\0")
        {
            var dialog = (IFileOpenDialog)new FileOpenDialog();
            try
            {
                var options = FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
                              FILEOPENDIALOGOPTIONS.FOS_NOVALIDATE |
                              FILEOPENDIALOGOPTIONS.FOS_FILEMUSTEXIST |
                              FILEOPENDIALOGOPTIONS.FOS_ALLOWMULTISELECT;
                dialog.SetOptions(options);

                if (!string.IsNullOrEmpty(title))
                    dialog.SetTitle(title);

                if (!string.IsNullOrEmpty(filter))
                {
                    var specs = MakeFilterSpec(filter);
                    dialog.SetFileTypes((uint)specs.Length, specs);
                }

                int hr = dialog.Show(IntPtr.Zero);
                if (hr != 0) return Array.Empty<string>(); // отмена

                hr = dialog.GetResults(out IShellItemArray items);
                if (hr != 0) return Array.Empty<string>();

                try
                {
                    items.GetCount(out uint count);
                    var paths = new string[count];
                    for (uint i = 0; i < count; i++)
                    {
                        items.GetItemAt(i, out IShellItem item);
                        try
                        {
                            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string path);
                            paths[i] = path;
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(item);
                        }
                    }
                    return paths;
                }
                finally
                {
                    Marshal.ReleaseComObject(items);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(dialog);
            }
        }

        /// <summary>
        /// Выбрать папку.
        /// </summary>
        public static string? SelectFolder(string title = "Select Folder")
        {
            var dialog = (IFileOpenDialog)new FileOpenDialog();
            try
            {
                var options = FILEOPENDIALOGOPTIONS.FOS_FORCEFILESYSTEM |
                              FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS;
                dialog.SetOptions(options);

                if (!string.IsNullOrEmpty(title))
                    dialog.SetTitle(title);

                int hr = dialog.Show(IntPtr.Zero);
                if (hr != 0) return null;

                hr = dialog.GetResult(out IShellItem item);
                if (hr != 0) return null;

                try
                {
                    item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string path);
                    return path;
                }
                finally
                {
                    Marshal.ReleaseComObject(item);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(dialog);
            }
        }

        private static COMDLG_FILTERSPEC[] MakeFilterSpec(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return Array.Empty<COMDLG_FILTERSPEC>();

            var parts = filter.Split('\0', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length % 2 != 0)
                throw new ArgumentException("Filter must consist of pairs: \"Description\\0*.ext\\0\"");

            var specs = new COMDLG_FILTERSPEC[parts.Length / 2];
            for (int i = 0; i < specs.Length; i++)
            {
                specs[i] = new COMDLG_FILTERSPEC
                {
                    pszName = parts[i * 2],
                    pszSpec = parts[i * 2 + 1]
                };
            }
            return specs;
        }

        #region === COM Interop (точные сигнатуры из Windows SDK) ===

        [ComImport, Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
        private class FileOpenDialog { }

        [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IFileOpenDialog
        {
            [PreserveSig] int Show(IntPtr parent);
            [PreserveSig] int SetFileTypes(uint cFileTypes, [MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);
            [PreserveSig] int SetFileTypeIndex(uint iFileType);
            [PreserveSig] int GetFileTypeIndex(out uint piFileType);
            [PreserveSig] int Advise(IntPtr pfde, out uint pdwCookie);
            [PreserveSig] int Unadvise(uint dwCookie);
            [PreserveSig] int SetOptions(FILEOPENDIALOGOPTIONS fos);
            [PreserveSig] int GetOptions(out FILEOPENDIALOGOPTIONS pfos);
            [PreserveSig] int SetDefaultFolder(IShellItem psi);
            [PreserveSig] int SetFolder(IShellItem psi);
            [PreserveSig] int GetFolder(out IShellItem ppsi);
            [PreserveSig] int GetCurrentSelection(out IShellItem ppsi);
            [PreserveSig] int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            [PreserveSig] int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
            [PreserveSig] int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
            [PreserveSig] int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
            [PreserveSig] int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
            [PreserveSig] int GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
            [PreserveSig] int AddPlace(IShellItem psi, FDAP fdap);
            [PreserveSig] int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
            [PreserveSig] int Close(int hr);
            [PreserveSig] int SetClientGuid(ref Guid guid);
            [PreserveSig] int ClearClientData();
            [PreserveSig] int SetFilter(IntPtr pFilter);
            [PreserveSig] int GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsia);
            [PreserveSig] int GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsia);
        }

        [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
            [PreserveSig] int BindToHandler(IntPtr pbc, ref Guid rbhid, ref Guid riid, out IntPtr ppv);
            [PreserveSig] int GetParent(out IShellItem ppsi);
            [PreserveSig] int GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
            [PreserveSig] int GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
            [PreserveSig] int Compare(IShellItem psi, uint hint, out int piOrder);
        }

        [ComImport, Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItemArray
        {
            [PreserveSig] int BindToHandler(IntPtr pbc, ref Guid rbhid, ref Guid riid, out IntPtr ppv);
            [PreserveSig] int GetPropertyStore(int flags, ref Guid riid, out IntPtr ppv);
            [PreserveSig] int GetPropertyDescriptionList(ref PROPERTYKEY keyType, ref Guid riid, out IntPtr ppv);
            [PreserveSig] int GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
            [PreserveSig] int GetCount(out uint pdwNumItems);
            [PreserveSig] int GetItemAt(uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
            [PreserveSig] int EnumItems(out IntPtr ppenumShellItems);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROPERTYKEY
        {
            public Guid fmtid;
            public uint pid;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)] public string pszName;
            [MarshalAs(UnmanagedType.LPWStr)] public string pszSpec;
        }

        [Flags]
        private enum FILEOPENDIALOGOPTIONS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040,
            FOS_ALLNONSTORAGEITEMS = 0x00000080,
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
        }

        private enum FDAP
        {
            FDAP_BOTTOM = 0,
            FDAP_TOP = 1
        }

        private enum SIGDN : uint
        {
            SIGDN_FILESYSPATH = 0x80058000
        }

        #endregion
    }
}
