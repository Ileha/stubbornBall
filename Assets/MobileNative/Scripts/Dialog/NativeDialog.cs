using UnityEngine;
using System.Collections;
using System;
using Cysharp.Threading.Tasks;

namespace pingak9
{
    public class NativeDialog
    {
        public NativeDialog()
        {
        }

        public static void OpenDialog(string title, string message, string ok = "Ok", Action okAction = null)
        {
            MobileDialogInfo.Create(title, message, ok, okAction);
        }

        public static void OpenDialog(
            string title, string message, string yes, string no, Action yesAction = null,
            Action noAction = null)
        {
            MobileDialogConfirm.Create(title, message, yes, no, yesAction, noAction);
        }

        public static void OpenDialog(
            string title, string message, string accept, string neutral, string decline,
            Action acceptAction = null, Action neutralAction = null, Action declineAction = null)
        {
            MobileDialogNeutral.Create(title, message, accept, neutral, decline, acceptAction, neutralAction,
                declineAction);
        }

        public static void OpenDatePicker(int year, int month, int day, bool cancelable = true,
            Action<DateTime> onChange = null, Action<DateTime> onClose = null, Action onDismiss = null)
        {
            MobileDateTimePicker.CreateDate(year, month, day, cancelable, onChange, onClose, onDismiss);
        }

        public static void OpenDatePicker(string message, int year, int month, int day, bool cancelable = true,
            Action<DateTime> onChange = null, Action<DateTime> onClose = null, Action onDismiss = null)
        {
            MobileDateTimePicker.CreateDate(message, year, month, day, cancelable, onChange, onClose, onDismiss);
        }

        public static void OpenTimePicker(Action<DateTime> onChange = null, Action<DateTime> onClose = null)
        {
            MobileDateTimePicker.CreateTime(onChange, onClose);
        }

        public static UniTask<DateTime> OpenDatePickerAsync(string message, int year, int month, int day,
            bool cancelable = true)
        {
            var source = new UniTaskCompletionSource<DateTime>();

            OpenDatePicker(
                message, year, month, day, cancelable,
                onClose: time => source.TrySetResult(time),
                onDismiss: () => source.TrySetCanceled()
                );

            return source.Task;
        }
    }
}