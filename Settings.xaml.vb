Imports Sudoku4U.SudokuGlobal
Imports System.IO

Partial Public Class Page1
    Inherits PhoneApplicationPage
    Dim appSettings As IsolatedStorage.IsolatedStorageSettings

    Public Sub New()
        InitializeComponent()
        appSettings = IsolatedStorage.IsolatedStorageSettings.ApplicationSettings
        If appSettings.Contains(PREDICTIVE_KEYBOARD) Then
            Dim val As String = ""
            appSettings.TryGetValue(PREDICTIVE_KEYBOARD, val)
            chkBoxPredictiveKeyboard.IsChecked = val
            'MessageBox.Show(chkBoxPredictiveKeyboard.IsChecked.ToString)
        Else
            chkBoxPredictiveKeyboard.IsChecked = PREDICTIVE_KEYBOARD_FLG
        End If
    End Sub

    Private Sub chkBoxPredictiveKeyboard_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles chkBoxPredictiveKeyboard.Checked, chkBoxPredictiveKeyboard.Unchecked
        Dim chkBox As New CheckBox
        chkBox = sender
        If chkBox.IsChecked Then
            PREDICTIVE_KEYBOARD_FLG = 1
        Else
            PREDICTIVE_KEYBOARD_FLG = 0
        End If
        If appSettings.Contains(PREDICTIVE_KEYBOARD) Then
            appSettings.Remove(PREDICTIVE_KEYBOARD)
        End If
        appSettings.Add(PREDICTIVE_KEYBOARD, PREDICTIVE_KEYBOARD_FLG)

    End Sub
End Class
