using System;
using System.Threading;
using NativeFileDialogSharp;

namespace RayKeys.Misc {
    public static class FileDialog {
        public static void OpenDialog(Action<DialogResult> finishFunction, string filterList = null, string defaultPath = null) {
            new Thread(() => {
                DialogResult result = Dialog.FileOpen(filterList, defaultPath);
                finishFunction.Invoke(result);
            }).Start();
        }
        
        public static void SaveDialog(Action<DialogResult> finishFunction, string filterList = null, string defaultPath = null) {
            new Thread(() => {
                DialogResult result = Dialog.FileSave(filterList, defaultPath);
                finishFunction.Invoke(result);
            }).Start();
        }
        
        public static void FolderDialog(Action<DialogResult> finishFunction, string defaultPath = null) {
            new Thread(() => {
                DialogResult result = Dialog.FolderPicker(defaultPath);
                finishFunction.Invoke(result);
            }).Start();
        }
        
        public static void MultiOpenFileDialog(Action<DialogResult> finishFunction, string filterList = null, string defaultPath = null) {
            new Thread(() => {
                DialogResult result = Dialog.FileOpenMultiple(filterList, defaultPath);
                finishFunction.Invoke(result);
            }).Start();
        }
    }
}