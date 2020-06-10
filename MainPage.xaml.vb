Imports Microsoft.Phone.Tasks
Imports System.IO.IsolatedStorage
Imports System.IO
Imports Sudoku4U.SudokuGlobal
Imports System.Threading

Partial Public Class MainPage
    Inherits PhoneApplicationPage

    'Inherits System.Windows.Forms.Form
    Private txtBox(9, 9) As TextBox
    Private btnNum(9) As Button
    'Private atLeastOneNumber As Boolean = False
    'Private sdk As New Sudoku
    Dim iSudoku(9, 9) As Integer
    Dim iSudokuSolution(9, 9) As Integer
    Dim isSeed(9, 9) As Integer
    Dim strSudoku As String = ""
    Dim puzzleInProgress As Boolean = False
    Dim appSettings As IsolatedStorage.IsolatedStorageSettings

    Dim tbIndexRow, tbIndexCol As Integer

    Dim mySolidSysColorBrush As New SolidColorBrush()
    Dim mySolidBlackColorBrush As New SolidColorBrush()
    Dim mySolidWhiteColorBrush As New SolidColorBrush()
    Dim mySolidRedColorBrush As New SolidColorBrush()
    Dim mySolidYellowColorBrush As New SolidColorBrush()

    ' Constructor
    Public Sub New()
        InitializeComponent()
        Init()
        LoadSudokus()
    End Sub
    Protected Overrides Sub OnBackKeyPress(e As System.ComponentModel.CancelEventArgs)
        'MessageBox.Show("On back key press")
        If (puzzleInProgress) Then
            Dim result As MessageBoxResult = MessageBox.Show("Do you want to save the current Sudoku?", "Warning", MessageBoxButton.OKCancel)
            If (result = MessageBoxResult.OK) Then
                saveSudoku()
            End If
        End If
        'saveSudoku()
        MyBase.OnBackKeyPress(e)
    End Sub
    Private Sub About_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.NavigationService.Navigate(New Uri("/About.xaml", UriKind.Relative))
    End Sub
    Private Sub ReviewItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim marketplaceReviewTask As MarketplaceReviewTask = New MarketplaceReviewTask()
        marketplaceReviewTask.Show()
    End Sub
    Private Sub LoadSudokus()
        'Dim settings As IsolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings

        Dim isFirstRun As Boolean = False
        Dim len As Integer = SUDOKUS.Length
        Dim i As Integer = 0
        Try
            If (Not appSettings.Contains(FIRST_RUN)) Then
                'First time running, so load Sudokus
                'MessageBox.Show(FIRST_RUN)

                'settings.Clear()
                Dim icurSudokuindex As Integer = 0
                If appSettings.Contains(CUR_SUDOKU_INDEX) Then
                    appSettings.TryGetValue(CUR_SUDOKU_INDEX, icurSudokuindex)
                Else
                    icurSudokuindex = INITIAL_SUDOKU_INDEX
                End If
                For i = icurSudokuindex To MAX_SUDOKUS2LOAD - 1 Step 1
                    If (appSettings.Contains(i.ToString)) Then
                        appSettings.Remove(i.ToString)
                    End If
                    appSettings.Add(i.ToString, SUDOKUS(i))
                Next

                If Not appSettings.Contains(CUR_SUDOKU_INDEX) Then
                    appSettings.Add(CUR_SUDOKU_INDEX, icurSudokuindex)
                End If
                appSettings.Add(FIRST_RUN, False)
                appSettings.Save()
            Else
                'MessageBox.Show("Not FirstRun")
                If appSettings.Contains(PREDICTIVE_KEYBOARD) Then
                    Dim val As String = ""
                    appSettings.TryGetValue(PREDICTIVE_KEYBOARD, val)
                    PREDICTIVE_KEYBOARD_FLG = val
                    'MessageBox.Show(chkBoxPredictiveKeyboard.IsChecked.ToString)
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub
    Private Sub Init()
        'btnSolve.IsEnabled = False
        'btnReset.IsEnabled = True
        btnSave.IsEnabled = False
        btnClear.IsEnabled = True
        btnNew.IsEnabled = True
        btnLoad.IsEnabled = True
        btnCheck.IsEnabled = False

        progBar.Visibility = Windows.Visibility.Collapsed

        appSettings = IsolatedStorageSettings.ApplicationSettings
        tbIndexRow = 0
        tbIndexCol = 0

        'atLeastOneNumber = False
        'Get current phone accent color
        mySolidSysColorBrush.Color = Application.Current.Resources("PhoneAccentColor")
        mySolidWhiteColorBrush.Color = Color.FromArgb(255, 255, 255, 255)
        mySolidBlackColorBrush.Color = Color.FromArgb(255, 0, 0, 0)
        mySolidRedColorBrush.Color = Color.FromArgb(255, 255, 0, 0)
        mySolidYellowColorBrush.Color = Color.FromArgb(255, 255, 255, 0)

        SupportedOrientations = SupportedPageOrientation.Portrait

        ' Add an Application Bar with a 'setting menu item.
        ApplicationBar = New ApplicationBar()
        ApplicationBar.IsMenuEnabled = True
        ApplicationBar.IsVisible = True
        ApplicationBar.Mode = ApplicationBarMode.Minimized
        ApplicationBar.Opacity = 1.0

        Dim hintItem As New ApplicationBarMenuItem("Hint")
        AddHandler hintItem.Click, AddressOf Hint_Click
        ApplicationBar.MenuItems.Add(hintItem)

        Dim solutionItem As New ApplicationBarMenuItem("Solve")
        AddHandler solutionItem.Click, AddressOf SolutionItem_Click
        ApplicationBar.MenuItems.Add(solutionItem)

        Dim aboutItem As New ApplicationBarMenuItem("About")
        AddHandler aboutItem.Click, AddressOf About_Click
        ApplicationBar.MenuItems.Add(aboutItem)

        Dim reviewItem As New ApplicationBarMenuItem("Review & Rate")
        AddHandler reviewItem.Click, AddressOf ReviewItem_Click
        ApplicationBar.MenuItems.Add(reviewItem)

        Dim settingsItem As New ApplicationBarMenuItem("Settings")
        AddHandler settingsItem.Click, AddressOf SettingsItem_Click
        ApplicationBar.MenuItems.Add(settingsItem)


        'Dim currentAccentBrushHex As Brush = Application.Current.Resources("PhoneAccentColor")

        txtBox(1, 1) = tb11
        txtBox(1, 2) = tb12
        txtBox(1, 3) = tb13
        txtBox(1, 4) = tb14
        txtBox(1, 5) = tb15
        txtBox(1, 6) = tb16
        txtBox(1, 7) = tb17
        txtBox(1, 8) = tb18
        txtBox(1, 9) = tb19

        txtBox(2, 1) = tb21
        txtBox(2, 2) = tb22
        txtBox(2, 3) = tb23
        txtBox(2, 4) = tb24
        txtBox(2, 5) = tb25
        txtBox(2, 6) = tb26
        txtBox(2, 7) = tb27
        txtBox(2, 8) = tb28
        txtBox(2, 9) = tb29

        txtBox(3, 1) = tb31
        txtBox(3, 2) = tb32
        txtBox(3, 3) = tb33
        txtBox(3, 4) = tb34
        txtBox(3, 5) = tb35
        txtBox(3, 6) = tb36
        txtBox(3, 7) = tb37
        txtBox(3, 8) = tb38
        txtBox(3, 9) = tb39

        txtBox(4, 1) = tb41
        txtBox(4, 2) = tb42
        txtBox(4, 3) = tb43
        txtBox(4, 4) = tb44
        txtBox(4, 5) = tb45
        txtBox(4, 6) = tb46
        txtBox(4, 7) = tb47
        txtBox(4, 8) = tb48
        txtBox(4, 9) = tb49

        txtBox(5, 1) = tb51
        txtBox(5, 2) = tb52
        txtBox(5, 3) = tb53
        txtBox(5, 4) = tb54
        txtBox(5, 5) = tb55
        txtBox(5, 6) = tb56
        txtBox(5, 7) = tb57
        txtBox(5, 8) = tb58
        txtBox(5, 9) = tb59

        txtBox(6, 1) = tb61
        txtBox(6, 2) = tb62
        txtBox(6, 3) = tb63
        txtBox(6, 4) = tb64
        txtBox(6, 5) = tb65
        txtBox(6, 6) = tb66
        txtBox(6, 7) = tb67
        txtBox(6, 8) = tb68
        txtBox(6, 9) = tb69

        txtBox(7, 1) = tb71
        txtBox(7, 2) = tb72
        txtBox(7, 3) = tb73
        txtBox(7, 4) = tb74
        txtBox(7, 5) = tb75
        txtBox(7, 6) = tb76
        txtBox(7, 7) = tb77
        txtBox(7, 8) = tb78
        txtBox(7, 9) = tb79

        txtBox(8, 1) = tb81
        txtBox(8, 2) = tb82
        txtBox(8, 3) = tb83
        txtBox(8, 4) = tb84
        txtBox(8, 5) = tb85
        txtBox(8, 6) = tb86
        txtBox(8, 7) = tb87
        txtBox(8, 8) = tb88
        txtBox(8, 9) = tb89

        txtBox(9, 1) = tb91
        txtBox(9, 2) = tb92
        txtBox(9, 3) = tb93
        txtBox(9, 4) = tb94
        txtBox(9, 5) = tb95
        txtBox(9, 6) = tb96
        txtBox(9, 7) = tb97
        txtBox(9, 8) = tb98
        txtBox(9, 9) = tb99

        btnNum(0) = btn0
        btnNum(1) = btn1
        btnNum(2) = btn2
        btnNum(3) = btn3
        btnNum(4) = btn4
        btnNum(5) = btn5
        btnNum(6) = btn6
        btnNum(7) = btn7
        btnNum(8) = btn8
        btnNum(9) = btn9



        Dim inputScope As InputScope = New InputScope
        Dim inputScopeName As InputScopeName = New InputScopeName
        'inputScopeName.NameValue = InputScopeNameValue.Digits
        inputScopeName.NameValue = InputScopeNameValue.Number

        inputScope.Names.Add(inputScopeName)

        'Assing all the visible controls with the current theme color
        Dim i, j As Integer
        For i = 1 To 9
            For j = 1 To 9
                txtBox(i, j).BorderBrush = mySolidSysColorBrush
                txtBox(i, j).Background = mySolidBlackColorBrush
                txtBox(i, j).Foreground = mySolidSysColorBrush
                txtBox(i, j).SelectionForeground = mySolidSysColorBrush
                txtBox(i, j).SelectionBackground = mySolidBlackColorBrush
                txtBox(i, j).InputScope = inputScope
            Next
            btnNum(i).IsEnabled = False
        Next
        btnNum(0).IsEnabled = False
        Border1.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border2.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border3.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border4.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border5.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border6.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border7.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border8.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border9.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        Border10.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush

        'btnReset.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        'btnReset.Foreground = mySolidSysColorBrush
        'btnSolve.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        'btnSolve.Foreground = mySolidSysColorBrush
        'btnSubmit.BorderBrush = mySolidWhiteColorBrush 'mySolidSysColorBrush
        'btnSubmit.Foreground = mySolidSysColorBrush

        txtBlockTitle.Foreground = mySolidSysColorBrush


        'Dim left As Integer = SudokuConst.CELL_HEIGHT
        'Dim top As Integer = SudokuConst.CELL_HEIGHT
        'Dim tabInd As Integer = 1

        ''Dim txtName As TextBox = New TextBox
        ''With txtName
        ''    .Text = "x"
        ''    'txtName.BorderThickness = New Thickness(10)
        ''    'txtName.Height = 20
        ''    .IsReadOnly = False
        ''    .MaxLength = 1
        ''    '.MaxWidth = 20
        ''End With
        ''Canvas1.Children.Add(txtName)

        'Grid1.ShowGridLines = True

        'For i = 1 To 3
        '    If (((i - 1) Mod 3 = 0) And ((i - 1) <> 0)) Then
        '        left = SudokuConst.CELL_HEIGHT
        '        top = top + SudokuConst.CELL_HEIGHT
        '    End If

        '    For j = 1 To 3

        '        If (((j - 1) Mod 3 = 0) And ((j - 1) <> 0)) Then
        '            left = left + SudokuConst.CELL_HEIGHT
        '        End If

        '        txtBox(i, j) = New TextBox
        '        With txtBox(i, j)
        '            .TabIndex = tabInd
        '            .Text = "0"
        '            .IsTabStop = True
        '            .TextAlignment = HorizontalAlignment.Center
        '            '.Margin = New System.Windows.Thickness(left, top, 0, 0)
        '            '.Left = left
        '            '.Top = top
        '            '.Width = SudokuConst.CELL_WIDTH
        '            '.Height = SudokuConst.CELL_HEIGHT
        '            .Visibility = Windows.Visibility.Visible
        '            .MaxLength = 1
        '            '.Visible = True
        '            '.Text = tabInd
        '            '.BorderThickness = New System.Windows.Thickness(1)
        '        End With
        '        left = left + SudokuConst.CELL_WIDTH
        '        tabInd = tabInd + 1
        '        'Canvas1.Children.Add(txtBox(i, j))
        '        'Me.Controls.Add(txtBox(i, j))
        '        Grid1.Children.Add(txtBox(i, j))
        '    Next j
        '    left = SudokuConst.CELL_WIDTH
        '    top = SudokuConst.CELL_HEIGHT
        'Next i
    End Sub

    Private Sub PhoneApplicationPage_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

    End Sub

    'Private Sub btnSubmit_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnSubmit.Click

    '    btnSolve.IsEnabled = False
    '    btnSubmit.IsEnabled = False

    '    Dim i, j As Integer
    '    For i = 1 To 9
    '        For j = 1 To 9
    '            If (txtBox(i, j).Text = "") Or (txtBox(i, j).Text = ".") Or (txtBox(i, j).Text = "0") Then
    '                iSudoku(i, j) = 0
    '            Else
    '                iSudoku(i, j) = Integer.Parse(txtBox(i, j).Text)
    '            End If
    '        Next
    '    Next



    '    'iSudoku(1, 1) = Integer.Parse(tb11.Text)
    '    'iSudoku(1, 2) = Integer.Parse(tb12.Text)
    '    'iSudoku(1, 3) = Integer.Parse(tb13.Text)
    '    'iSudoku(1, 4) = Integer.Parse(tb14.Text)
    '    'iSudoku(1, 5) = Integer.Parse(tb15.Text)
    '    'iSudoku(1, 6) = Integer.Parse(tb16.Text)
    '    'iSudoku(1, 7) = Integer.Parse(tb17.Text)
    '    'iSudoku(1, 8) = Integer.Parse(tb18.Text)
    '    'iSudoku(1, 9) = Integer.Parse(tb19.Text)

    '    'iSudoku(2, 1) = Integer.Parse(tb21.Text)
    '    'iSudoku(2, 2) = Integer.Parse(tb22.Text)
    '    'iSudoku(2, 3) = Integer.Parse(tb23.Text)
    '    'iSudoku(2, 4) = Integer.Parse(tb24.Text)
    '    'iSudoku(2, 5) = Integer.Parse(tb25.Text)
    '    'iSudoku(2, 6) = Integer.Parse(tb26.Text)
    '    'iSudoku(2, 7) = Integer.Parse(tb27.Text)
    '    'iSudoku(2, 8) = Integer.Parse(tb28.Text)
    '    'iSudoku(2, 9) = Integer.Parse(tb29.Text)

    '    'iSudoku(3, 1) = Integer.Parse(tb31.Text)
    '    'iSudoku(3, 2) = Integer.Parse(tb32.Text)
    '    'iSudoku(3, 3) = Integer.Parse(tb33.Text)
    '    'iSudoku(3, 4) = Integer.Parse(tb34.Text)
    '    'iSudoku(3, 5) = Integer.Parse(tb35.Text)
    '    'iSudoku(3, 6) = Integer.Parse(tb36.Text)
    '    'iSudoku(3, 7) = Integer.Parse(tb37.Text)
    '    'iSudoku(3, 8) = Integer.Parse(tb38.Text)
    '    'iSudoku(3, 9) = Integer.Parse(tb39.Text)

    '    'iSudoku(4, 1) = Integer.Parse(tb41.Text)
    '    'iSudoku(4, 2) = Integer.Parse(tb42.Text)
    '    'iSudoku(4, 3) = Integer.Parse(tb43.Text)
    '    'iSudoku(4, 4) = Integer.Parse(tb44.Text)
    '    'iSudoku(4, 5) = Integer.Parse(tb45.Text)
    '    'iSudoku(4, 6) = Integer.Parse(tb46.Text)
    '    'iSudoku(4, 7) = Integer.Parse(tb47.Text)
    '    'iSudoku(4, 8) = Integer.Parse(tb48.Text)
    '    'iSudoku(4, 9) = Integer.Parse(tb49.Text)

    '    'iSudoku(5, 1) = Integer.Parse(tb51.Text)
    '    'iSudoku(5, 2) = Integer.Parse(tb52.Text)
    '    'iSudoku(5, 3) = Integer.Parse(tb53.Text)
    '    'iSudoku(5, 4) = Integer.Parse(tb54.Text)
    '    'iSudoku(5, 5) = Integer.Parse(tb55.Text)
    '    'iSudoku(5, 6) = Integer.Parse(tb56.Text)
    '    'iSudoku(5, 7) = Integer.Parse(tb57.Text)
    '    'iSudoku(5, 8) = Integer.Parse(tb58.Text)
    '    'iSudoku(5, 9) = Integer.Parse(tb59.Text)

    '    'iSudoku(6, 1) = Integer.Parse(tb61.Text)
    '    'iSudoku(6, 2) = Integer.Parse(tb62.Text)
    '    'iSudoku(6, 3) = Integer.Parse(tb63.Text)
    '    'iSudoku(6, 4) = Integer.Parse(tb64.Text)
    '    'iSudoku(6, 5) = Integer.Parse(tb65.Text)
    '    'iSudoku(6, 6) = Integer.Parse(tb66.Text)
    '    'iSudoku(6, 7) = Integer.Parse(tb67.Text)
    '    'iSudoku(6, 8) = Integer.Parse(tb68.Text)
    '    'iSudoku(6, 9) = Integer.Parse(tb69.Text)

    '    'iSudoku(7, 1) = Integer.Parse(tb71.Text)
    '    'iSudoku(7, 2) = Integer.Parse(tb72.Text)
    '    'iSudoku(7, 3) = Integer.Parse(tb73.Text)
    '    'iSudoku(7, 4) = Integer.Parse(tb74.Text)
    '    'iSudoku(7, 5) = Integer.Parse(tb75.Text)
    '    'iSudoku(7, 6) = Integer.Parse(tb76.Text)
    '    'iSudoku(7, 7) = Integer.Parse(tb77.Text)
    '    'iSudoku(7, 8) = Integer.Parse(tb78.Text)
    '    'iSudoku(7, 9) = Integer.Parse(tb79.Text)

    '    'iSudoku(8, 1) = Integer.Parse(tb81.Text)
    '    'iSudoku(8, 2) = Integer.Parse(tb82.Text)
    '    'iSudoku(8, 3) = Integer.Parse(tb83.Text)
    '    'iSudoku(8, 4) = Integer.Parse(tb84.Text)
    '    'iSudoku(8, 5) = Integer.Parse(tb85.Text)
    '    'iSudoku(8, 6) = Integer.Parse(tb86.Text)
    '    'iSudoku(8, 7) = Integer.Parse(tb87.Text)
    '    'iSudoku(8, 8) = Integer.Parse(tb88.Text)
    '    'iSudoku(8, 9) = Integer.Parse(tb89.Text)

    '    'iSudoku(9, 1) = Integer.Parse(tb91.Text)
    '    'iSudoku(9, 2) = Integer.Parse(tb92.Text)
    '    'iSudoku(9, 3) = Integer.Parse(tb93.Text)
    '    'iSudoku(9, 4) = Integer.Parse(tb94.Text)
    '    'iSudoku(9, 5) = Integer.Parse(tb95.Text)
    '    'iSudoku(9, 6) = Integer.Parse(tb96.Text)
    '    'iSudoku(9, 7) = Integer.Parse(tb97.Text)
    '    'iSudoku(9, 8) = Integer.Parse(tb98.Text)
    '    'iSudoku(9, 9) = Integer.Parse(tb99.Text)

    '    fillSudoku()
    '    'NavigationService.Navigate(New Uri("/Page1.xaml", UriKind.Relative))

    'End Sub
    Private Sub fillSudoku(ByRef sdk As Sudoku)
        'If (Not atLeastOneNumber) Then
        '    sdk.flush()
        '    txtBlockMsg.Text = "Please fill atleast one cell. Don't try to crash me!"
        '    txtBlockMsg.Foreground = mySolidRedColorBrush
        '    btnSubmit.IsEnabled = True
        '    Exit Sub
        'End If

        Dim i, j, p, q As Integer
        p = 1
        q = 1

        Dim a, b, c, d As Integer
        'Dim atLeatOneNumber As Boolean = False
        a = 1
        c = 1
        For i = 1 To 9
            b = 1
            d = 1
            If ((i - 1) Mod 3 = 0) And (i <> 1) Then
                a = a + 1
                If (a > 3) Then
                    a = 1
                End If
            End If

            For j = 1 To 9
                sdk.cel(i, j).value = iSudoku(i, j)

                If (sdk.cel(i, j).value > 0) Then
                    sdk.cel(i, j).isSeed = True
                    sdk.cel(i, j).isFilled = True
                    sdk.filledCount = sdk.filledCount + 1
                End If

                sdk.cel(i, j).RowId = i
                sdk.cel(i, j).ColId = j

                sdk.cel(i, j).rgnRowId = a

                If ((j - 1) Mod 3 = 0) And (j <> 1) Then
                    b = b + 1
                    If (b > 3) Then
                        b = 1
                    End If
                End If
                sdk.cel(i, j).rgncolId = b

                sdk.cel(i, j).inRgnRowId = c

                sdk.cel(i, j).inRgnColId = d
                d = d + 1
                If (d > 3) Then
                    d = 1
                End If

            Next j

            c = c + 1
            If (c > 3) Then
                c = 1
            End If
        Next i

        'remove this once done
        'sdk.cel(1, 1).assignVal(5, True)
        'sdk.cel(1, 2).assignVal(7, True)
        'sdk.cel(1, 3).assignVal(1, True)
        'sdk.cel(1, 6).assignVal(9, True)
        'sdk.cel(2, 1).assignVal(3, True)
        'sdk.cel(2, 5).assignVal(5, True)
        'sdk.cel(2, 7).assignVal(9, True)
        'sdk.cel(2, 9).assignVal(7, True)
        'sdk.cel(3, 1).assignVal(9, True)
        'sdk.cel(3, 4).assignVal(7, True)
        'sdk.cel(3, 9).assignVal(6, True)
        'sdk.cel(4, 3).assignVal(2, True)
        'sdk.cel(4, 6).assignVal(5, True)
        'sdk.cel(4, 7).assignVal(6, True)
        'sdk.cel(4, 8).assignVal(9, True)
        'sdk.cel(4, 9).assignVal(3, True)
        'sdk.cel(5, 2).assignVal(5, True)
        'sdk.cel(5, 6).assignVal(8, True)
        'sdk.cel(5, 7).assignVal(7, True)
        'sdk.cel(6, 1).assignVal(1, True)
        'sdk.cel(6, 4).assignVal(3, True)
        'sdk.cel(6, 5).assignVal(6, True)
        'sdk.cel(6, 8).assignVal(8, True)
        'sdk.cel(7, 2).assignVal(4, True)
        'sdk.cel(7, 4).assignVal(2, True)
        'sdk.cel(7, 5).assignVal(1, True)
        'sdk.cel(8, 4).assignVal(8, True)
        'sdk.cel(8, 6).assignVal(6, True)
        'sdk.cel(8, 8).assignVal(7, True)
        'sdk.cel(8, 9).assignVal(9, True)
        'sdk.cel(9, 2).assignVal(3, True)
        'sdk.cel(9, 3).assignVal(6, True)
        'sdk.cel(9, 4).assignVal(5, True)
        'sdk.cel(9, 8).assignVal(2, True)

        'sdk.filledCount = 34

        'sdk.cel(4, 4).assignVal(9, True)
        'sdk.cel(6, 6).assignVal(9, True)
        'sdk.filledCount = 2

        sdk.fillRegions()

        'prints in a string
        'sdk.print()

        'Not requored for this app
        'For i = 1 To 9
        '    For j = 1 To 9
        '        If sdk.cel(i, j).value > 0 Then
        '            If sdk.isTwiceInColumn(sdk.cel(i, j)) Or
        '               sdk.isTwiceInRow(sdk.cel(i, j)) Or
        '                sdk.isTwiceInRegn(sdk.cel(i, j).rgnRowId, sdk.cel(i, j).rgncolId, sdk.cel(i, j)) Then
        '                txtBlockMsg.Text = "No repetitions allowed in a column/row/region!!"
        '                txtBlockMsg.Foreground = mySolidRedColorBrush
        '                Exit Sub
        '            End If
        '        End If
        '    Next
        'Next

        'sdk.print()
        'btnSolve.IsEnabled = True
        'Lock the input boxes
        'For i = 1 To 9
        '    For j = 1 To 9
        'txtBox(i, j).IsReadOnly = True
        'Next
        'Next

    End Sub

    Private Sub tb99_GotFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles tb99.GotFocus, tb98.GotFocus, tb97.GotFocus, tb96.GotFocus, tb95.GotFocus, tb94.GotFocus, tb93.GotFocus, tb92.GotFocus, tb91.GotFocus, tb89.GotFocus, tb88.GotFocus, tb87.GotFocus, tb86.GotFocus, tb85.GotFocus, tb84.GotFocus, tb83.GotFocus, tb82.GotFocus, tb81.GotFocus, tb79.GotFocus, tb78.GotFocus, tb77.GotFocus, tb76.GotFocus, tb75.GotFocus, tb74.GotFocus, tb73.GotFocus, tb72.GotFocus, tb71.GotFocus, tb69.GotFocus, tb68.GotFocus, tb67.GotFocus, tb66.GotFocus, tb65.GotFocus, tb64.GotFocus, tb63.GotFocus, tb62.GotFocus, tb61.GotFocus, tb59.GotFocus, tb58.GotFocus, tb57.GotFocus, tb56.GotFocus, tb55.GotFocus, tb54.GotFocus, tb53.GotFocus, tb52.GotFocus, tb51.GotFocus, tb49.GotFocus, tb48.GotFocus, tb47.GotFocus, tb46.GotFocus, tb45.GotFocus, tb44.GotFocus, tb43.GotFocus, tb42.GotFocus, tb41.GotFocus, tb39.GotFocus, tb38.GotFocus, tb37.GotFocus, tb36.GotFocus, tb35.GotFocus, tb34.GotFocus, tb33.GotFocus, tb32.GotFocus, tb31.GotFocus, tb29.GotFocus, tb28.GotFocus, tb27.GotFocus, tb26.GotFocus, tb25.GotFocus, tb24.GotFocus, tb23.GotFocus, tb22.GotFocus, tb21.GotFocus, tb19.GotFocus, tb18.GotFocus, tb17.GotFocus, tb16.GotFocus, tb15.GotFocus, tb14.GotFocus, tb13.GotFocus, tb12.GotFocus, tb11.GotFocus
        btnClear.Focus()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim txtBx As TextBox = sender

        If (puzzleInProgress) Then
            'Remove the yello color from previously highlighted box
            If (Not tbIndexRow = 0) Or (Not tbIndexCol = 0) Then
                txtBox(tbIndexRow, tbIndexCol).BorderBrush = mySolidSysColorBrush
            End If

            'Highlight the current selected box
            txtBx.BorderBrush = mySolidYellowColorBrush

            tbIndexRow = Integer.Parse(txtBx.Name.Chars(txtBx.Name.Length - 2))
            tbIndexCol = Integer.Parse(txtBx.Name.Chars(txtBx.Name.Length - 1))

            If (isSeed(tbIndexRow, tbIndexCol) = 1) Then
                For i = 0 To 9
                    btnNum(i).IsEnabled = False
                Next
            Else
                For i = 0 To 9
                    btnNum(i).IsEnabled = True
                Next
            End If

            If (PREDICTIVE_KEYBOARD_FLG) Then
                If (puzzleInProgress) Then
                    'Enable all the button, then disable them as needed
                    For i = 1 To 9
                        btnNum(i).IsEnabled = True
                    Next

                    If (Not txtBx.Text = "") Then
                        If (isSeed(tbIndexRow, tbIndexCol) = 1) Then
                            For i = 0 To 9
                                btnNum(i).IsEnabled = False
                            Next
                        Else
                            'Not blank and not seed.
                            btn0.IsEnabled = True
                            For i = 1 To 9
                                btnNum(i).IsEnabled = False
                            Next
                        End If
                    Else
                        For i = 1 To 9 'Number
                            If (Not txtBox(i, tbIndexCol).Text = "") Then
                                btnNum(Integer.Parse(txtBox(i, tbIndexCol).Text)).IsEnabled = False
                            End If
                            If (Not txtBox(tbIndexRow, i).Text = "") Then
                                btnNum(Integer.Parse(txtBox(tbIndexRow, i).Text)).IsEnabled = False
                            End If
                        Next
                        'Çheck regions
                        If (tbIndexRow <= 3) Then
                            i = 1
                        ElseIf (tbIndexRow > 3) And (tbIndexRow <= 6) Then
                            i = 4
                        ElseIf (tbIndexRow > 6) And (tbIndexRow <= 9) Then
                            i = 7
                        End If
                        If (tbIndexCol <= 3) Then
                            j = 1
                        ElseIf (tbIndexCol > 3) And (tbIndexCol <= 6) Then
                            j = 4
                        ElseIf (tbIndexCol > 6) And (tbIndexCol <= 9) Then
                            j = 7
                        End If
                        Dim p, q As Integer
                        For p = i To i + 2
                            For q = j To j + 2
                                If (Not txtBox(p, q).Text = "") Then
                                    btnNum(Integer.Parse(txtBox(p, q).Text)).IsEnabled = False
                                End If
                            Next
                        Next
                    End If
                End If
            End If 'End of PREDICTIVE_KEYBOARD_FLG condtion
        End If
    End Sub

    Private Sub tb99_TextInput(sender As System.Object, e As System.Windows.Input.TextCompositionEventArgs) Handles tb99.TextInput, tb98.TextInput, tb97.TextInput, tb96.TextInput, tb95.TextInput, tb94.TextInput, tb93.TextInput, tb92.TextInput, tb91.TextInput, tb89.TextInput, tb88.TextInput, tb87.TextInput, tb86.TextInput, tb85.TextInput, tb84.TextInput, tb83.TextInput, tb82.TextInput, tb81.TextInput, tb79.TextInput, tb78.TextInput, tb77.TextInput, tb76.TextInput, tb75.TextInput, tb74.TextInput, tb73.TextInput, tb72.TextInput, tb71.TextInput, tb69.TextInput, tb68.TextInput, tb67.TextInput, tb66.TextInput, tb65.TextInput, tb64.TextInput, tb63.TextInput, tb62.TextInput, tb61.TextInput, tb59.TextInput, tb58.TextInput, tb57.TextInput, tb56.TextInput, tb55.TextInput, tb54.TextInput, tb53.TextInput, tb52.TextInput, tb51.TextInput, tb49.TextInput, tb48.TextInput, tb47.TextInput, tb46.TextInput, tb45.TextInput, tb44.TextInput, tb43.TextInput, tb42.TextInput, tb41.TextInput, tb39.TextInput, tb38.TextInput, tb37.TextInput, tb36.TextInput, tb35.TextInput, tb34.TextInput, tb33.TextInput, tb32.TextInput, tb31.TextInput, tb29.TextInput, tb28.TextInput, tb27.TextInput, tb26.TextInput, tb25.TextInput, tb24.TextInput, tb23.TextInput, tb22.TextInput, tb21.TextInput, tb19.TextInput, tb18.TextInput, tb17.TextInput, tb16.TextInput, tb15.TextInput, tb14.TextInput, tb13.TextInput, tb12.TextInput, tb11.TextInput

        'Dim txtBox As TextBox = sender


    End Sub

    Private Sub tb99_Tap(sender As System.Object, e As System.Windows.Input.GestureEventArgs) Handles tb99.Tap, tb98.Tap, tb97.Tap, tb96.Tap, tb95.Tap, tb94.Tap, tb93.Tap, tb92.Tap, tb91.Tap, tb89.Tap, tb88.Tap, tb87.Tap, tb86.Tap, tb85.Tap, tb84.Tap, tb83.Tap, tb82.Tap, tb81.Tap, tb79.Tap, tb78.Tap, tb77.Tap, tb76.Tap, tb75.Tap, tb74.Tap, tb73.Tap, tb72.Tap, tb71.Tap, tb69.Tap, tb68.Tap, tb67.Tap, tb66.Tap, tb65.Tap, tb64.Tap, tb63.Tap, tb62.Tap, tb61.Tap, tb59.Tap, tb58.Tap, tb57.Tap, tb56.Tap, tb55.Tap, tb54.Tap, tb53.Tap, tb52.Tap, tb51.Tap, tb49.Tap, tb48.Tap, tb47.Tap, tb46.Tap, tb45.Tap, tb44.Tap, tb43.Tap, tb42.Tap, tb41.Tap, tb39.Tap, tb38.Tap, tb37.Tap, tb36.Tap, tb35.Tap, tb34.Tap, tb33.Tap, tb32.Tap, tb31.Tap, tb29.Tap, tb28.Tap, tb27.Tap, tb26.Tap, tb25.Tap, tb24.Tap, tb23.Tap, tb22.Tap, tb21.Tap, tb19.Tap, tb18.Tap, tb17.Tap, tb16.Tap, tb15.Tap, tb14.Tap, tb13.Tap, tb12.Tap, tb11.Tap
        Dim txtBox As TextBox = sender
        txtBox.SelectAll()
    End Sub

    'Private Sub tb99_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles tb99.TextChanged, tb98.TextChanged, tb97.TextChanged, tb96.TextChanged, tb95.TextChanged, tb94.TextChanged, tb93.TextChanged, tb92.TextChanged, tb91.TextChanged, tb89.TextChanged, tb88.TextChanged, tb87.TextChanged, tb86.TextChanged, tb85.TextChanged, tb84.TextChanged, tb83.TextChanged, tb82.TextChanged, tb81.TextChanged, tb79.TextChanged, tb78.TextChanged, tb77.TextChanged, tb76.TextChanged, tb75.TextChanged, tb74.TextChanged, tb73.TextChanged, tb72.TextChanged, tb71.TextChanged, tb69.TextChanged, tb68.TextChanged, tb67.TextChanged, tb66.TextChanged, tb65.TextChanged, tb64.TextChanged, tb63.TextChanged, tb62.TextChanged, tb61.TextChanged, tb59.TextChanged, tb58.TextChanged, tb57.TextChanged, tb56.TextChanged, tb55.TextChanged, tb54.TextChanged, tb53.TextChanged, tb52.TextChanged, tb51.TextChanged, tb49.TextChanged, tb48.TextChanged, tb47.TextChanged, tb46.TextChanged, tb45.TextChanged, tb44.TextChanged, tb43.TextChanged, tb42.TextChanged, tb41.TextChanged, tb39.TextChanged, tb38.TextChanged, tb37.TextChanged, tb36.TextChanged, tb35.TextChanged, tb34.TextChanged, tb33.TextChanged, tb32.TextChanged, tb31.TextChanged, tb29.TextChanged, tb28.TextChanged, tb27.TextChanged, tb26.TextChanged, tb25.TextChanged, tb24.TextChanged, tb23.TextChanged, tb22.TextChanged, tb21.TextChanged, tb19.TextChanged, tb18.TextChanged, tb17.TextChanged, tb16.TextChanged, tb15.TextChanged, tb14.TextChanged, tb13.TextChanged, tb12.TextChanged, tb11.TextChanged
    '    btnSubmit.IsEnabled = True
    '    Dim txtBox As TextBox = sender

    '    'MessageBox.Show(txtBox.ToString)

    'End Sub

    'Private Sub btnSolve_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)


    '    sdk.solve()
    '    'sdk.print()

    '    'MessageBox.Show("Please rate the application from the About screen, if you like it..!!", "Sudoku is Solved", MessageBoxButton.OK)

    '    For i = 1 To 9
    '        For j = 1 To 9
    '            'txtBox(i, j).IsReadOnly = False

    '            If (txtBox(i, j).Text = "") Then
    '                txtBox(i, j).Text = sdk.cel(i, j).value
    '                'Dim mySolidColorBrush As New SolidColorBrush()
    '                'mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, 255)
    '                txtBox(i, j).Foreground = mySolidWhiteColorBrush

    '            End If
    '        Next
    '    Next

    '    sdk.flush()
    '    'btnSolve.IsEnabled = False

    'End Sub

    Private Sub tb99_LostFocus(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles tb99.LostFocus, tb98.LostFocus, tb97.LostFocus, tb96.LostFocus, tb95.LostFocus, tb94.LostFocus, tb93.LostFocus, tb92.LostFocus, tb91.LostFocus, tb89.LostFocus, tb88.LostFocus, tb87.LostFocus, tb86.LostFocus, tb85.LostFocus, tb84.LostFocus, tb83.LostFocus, tb82.LostFocus, tb81.LostFocus, tb79.LostFocus, tb78.LostFocus, tb77.LostFocus, tb76.LostFocus, tb75.LostFocus, tb74.LostFocus, tb73.LostFocus, tb72.LostFocus, tb71.LostFocus, tb69.LostFocus, tb68.LostFocus, tb67.LostFocus, tb66.LostFocus, tb65.LostFocus, tb64.LostFocus, tb63.LostFocus, tb62.LostFocus, tb61.LostFocus, tb59.LostFocus, tb58.LostFocus, tb57.LostFocus, tb56.LostFocus, tb55.LostFocus, tb54.LostFocus, tb53.LostFocus, tb52.LostFocus, tb51.LostFocus, tb49.LostFocus, tb48.LostFocus, tb47.LostFocus, tb46.LostFocus, tb45.LostFocus, tb44.LostFocus, tb43.LostFocus, tb42.LostFocus, tb41.LostFocus, tb39.LostFocus, tb38.LostFocus, tb37.LostFocus, tb36.LostFocus, tb35.LostFocus, tb34.LostFocus, tb33.LostFocus, tb32.LostFocus, tb31.LostFocus, tb29.LostFocus, tb28.LostFocus, tb27.LostFocus, tb26.LostFocus, tb25.LostFocus, tb24.LostFocus, tb23.LostFocus, tb22.LostFocus, tb21.LostFocus, tb19.LostFocus, tb18.LostFocus, tb17.LostFocus, tb16.LostFocus, tb15.LostFocus, tb14.LostFocus, tb13.LostFocus, tb12.LostFocus, tb11.LostFocus
        Dim txtBox As TextBox = sender

        'Dim textIsNumeric As Boolean = True
        'Try
        '    Dim num = Integer.Parse(txtBox.Text)
        '    If (num > 0) And (num <= 9) Then
        '        'atLeastOneNumber = True
        '        btnSave.IsEnabled = True
        '    ElseIf (Integer.Parse(txtBox.Text) = 0) Then
        '        Throw New Exception("")
        '    End If
        'Catch
        '    'txtBlockMsg.Text = "Don't Just play around Buddy..!! All the values must be number > 1 and <=9!!"
        '    'txtBlockMsg.Foreground = mySolidRedColorBrush
        '    'MessageBox.Show("0 is not allowed", "Warning", MessageBoxButton.OK)
        '    txtBox.Text = ""
        '    txtBox.SelectAll()
        'End Try

        'Check if the Seed is changed
        'Dim i As Integer = Integer.Parse(txtBox.Name.Chars(txtBox.Name.Length - 2))
        'Dim j As Integer = Integer.Parse(txtBox.Name.Chars(txtBox.Name.Length - 1))
        'If (isSeed(i, j) = 1) Then
        '    If (Not iSudoku(i, j) = Integer.Parse(txtBox.Text)) Then
        '        txtBox.Text = iSudoku(i, j).ToString
        '        MessageBox.Show("Seed Value should not be changed", "Error", MessageBoxButton.OK)
        '    End If
        'End If

    End Sub

    'Private Sub btnReset_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnReset.Click
    '    Dim msgBoxResult As New MessageBoxResult

    '    msgBoxResult = MessageBox.Show("Do you want to reset the puzzle?", "Reset Confirmation", MessageBoxButton.OKCancel)

    '    If (msgBoxResult = MessageBoxResult.OK) Then
    '        sdk.flush()
    '        Dim mySolidColorBrush As New SolidColorBrush()
    '        Dim i, j As Integer
    '        For i = 1 To 9
    '            For j = 1 To 9
    '                iSudoku(i, j) = 0
    '                'txtBox(i, j).IsReadOnly = False
    '                txtBox(i, j).Text = ""
    '                mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, 0)
    '                txtBox(i, j).Foreground = mySolidSysColorBrush 'mySolidWhiteColorBrush
    '            Next
    '        Next
    '        txtBox(1, 1).SelectAll()
    '        txtBlockMsg.Foreground = mySolidWhiteColorBrush
    '        txtBlockMsg.Text = "Enter Sudoku puzzle & click Submit."
    '        'atLeastOneNumber = False
    '        'btnSolve.IsEnabled = False
    '        'btnSubmit.IsEnabled = True

    '        'Dim settings As IsolatedStorage.IsolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings
    '        appSettings.Clear()
    '    End If
    'End Sub

    Private Sub btnNew_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnNew.Click
        'Dim settings = IsolatedStorageSettings.ApplicationSettings
        'settings.Clear()
        Dim sdk As New Sudoku

        Dim i As Integer
        Dim p As Integer = 0
        Dim q As Integer = 0
        Dim r As Integer = 0

        puzzleInProgress = True
        btnNew.IsEnabled = False
        btnClear.IsEnabled = True
        btnLoad.IsEnabled = False
        btnSave.IsEnabled = True
        btnCheck.IsEnabled = True
        Try
            'MessageBox.Show(settings.Count.ToString)

            appSettings.TryGetValue(CUR_SUDOKU_INDEX, i)
            appSettings.Remove(CUR_SUDOKU_INDEX)
            appSettings.TryGetValue(i.ToString, strSudoku)
            'settings.Remove(i.ToString)
            'Move the current sudoku to next index value
            i = i + 1
            If (i = MAX_SUDOKUS2LOAD) Then
                i = INITIAL_SUDOKU_INDEX
                MessageBox.Show("Please update the app from MarketStore for more puzzles!", "Warning", MessageBoxButton.OK)
            End If
            appSettings.Add(CUR_SUDOKU_INDEX, i)
            appSettings.Save()

            For p = 1 To 9 Step 1
                For q = 1 To 9 Step 1
                    txtBox(p, q).BorderBrush = mySolidSysColorBrush
                    If (Not strSudoku.Chars(r) = "0") Then
                        txtBox(p, q).Text = strSudoku.Chars(r)
                        iSudoku(p, q) = Integer.Parse(strSudoku.Chars(r))
                        isSeed(p, q) = 1
                        txtBox(p, q).Foreground = mySolidWhiteColorBrush
                    Else
                        iSudoku(p, q) = 0
                        isSeed(p, q) = 0
                    End If
                    r = r + 1
                Next
                btnNum(p).IsEnabled = True
            Next
            btn0.IsEnabled = True
        Catch ex As Exception
            MessageBox.Show("No More Sudokus..!!")
            Exit Sub
        End Try

        'progBar.IsEnabled = True
        'progBar.Visibility = Windows.Visibility.Visible

        'Solve sudoku in a separate thread. Not actually required.
        Dim thread As New Thread(AddressOf mySudokuSolver)
        thread.Start(sdk)

        thread.Join()
        For p = 1 To 9 Step 1
            For q = 1 To 9 Step 1
                iSudokuSolution(p, q) = sdk.cel(p, q).value
            Next
        Next
        'progBar.Visibility = Windows.Visibility.Collapsed
        'MessageBox.Show(iSudokuSolution.ToString)

    End Sub

    Private Sub btnLoad_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnLoad.Click
        Dim i, j, x As Integer
        Dim savedSudoku As SavedSudoku = Nothing

        If (puzzleInProgress) Then
            Dim msgBoxReturn As MessageBoxResult = MessageBox.Show("A puzzle is in progress. Do you wish to discard?", "Warning", MessageBoxButton.OKCancel)
            If (msgBoxReturn = MessageBoxResult.Cancel) Then
                Exit Sub
            End If
        End If

        Try
            If (appSettings.Contains(SAVED_SUDOKU) = False) Then
                MessageBox.Show("No saved Sudoku!", "Warning", MessageBoxButton.OK)
                Exit Sub
            Else
                appSettings.TryGetValue(SAVED_SUDOKU, savedSudoku)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        x = 0
        For i = 1 To 9
            For j = 1 To 9
                If (savedSudoku.strSudoku.Chars(x) = "0") Then
                    txtBox(i, j).Text = ""
                Else
                    txtBox(i, j).Text = savedSudoku.strSudoku.Chars(x)
                End If
                'Load the Saved Solution as well
                iSudokuSolution(i, j) = Integer.Parse(savedSudoku.strSudokuSolution.Chars(x))
                isSeed(i, j) = Integer.Parse(savedSudoku.strSudokuSeeds.Chars(x))
                If (isSeed(i, j) = 1) Then
                    txtBox(i, j).Foreground = mySolidWhiteColorBrush
                End If
                x = x + 1
            Next
            btnNum(i).IsEnabled = True
        Next
        btn0.IsEnabled = True
        MessageBox.Show("Saved Sudoku is loaded", "Info", MessageBoxButton.OK)

        puzzleInProgress = True
        btnClear.IsEnabled = True
        btnNew.IsEnabled = False
        btnLoad.IsEnabled = False
        btnCheck.IsEnabled = True
        btnSave.IsEnabled = True

    End Sub

    Private Sub btnClear_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnClear.Click
        Dim p, q As Integer

        If (puzzleInProgress) Then
            Dim msgBoxRes As MessageBoxResult = MessageBox.Show("A Sudoku is in progress. Do you wish to discard?", "Warning", MessageBoxButton.OKCancel)
            If (Not msgBoxRes = MessageBoxResult.OK) Then
                Exit Sub
            End If
        End If

        For p = 1 To 9 Step 1
            For q = 1 To 9 Step 1
                txtBox(p, q).Text = ""
                txtBox(p, q).Foreground = mySolidSysColorBrush
                txtBox(p, q).BorderBrush = mySolidSysColorBrush
                isSeed(p, q) = 0
                iSudokuSolution(p, q) = 0
                iSudoku(p, q) = 0
            Next
            btnNum(p).IsEnabled = False
        Next
        btn0.IsEnabled = False
        puzzleInProgress = False
        btnNew.IsEnabled = True
        btnSave.IsEnabled = False
        btnLoad.IsEnabled = True
        btnCheck.IsEnabled = False
    End Sub

    Private Sub btnSave_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        saveSudoku()
        MessageBox.Show("Current Sudoku is saved", "Info", MessageBoxButton.OK)
    End Sub
    Private Sub saveSudoku()
        Dim i, j As Integer
        Dim savedSudoku As New SavedSudoku

        For i = 1 To 9
            For j = 1 To 9
                If (txtBox(i, j).Text.Trim = "") Then
                    savedSudoku.strSudoku += "0"
                Else
                    savedSudoku.strSudoku += txtBox(i, j).Text.Trim
                End If
                'If (isSeed(i, j)) Then
                '    savedSudoku.isSeed(i, j) = True
                'Else
                '    savedSudoku.isSeed(i, j) = False
                'End If
                'Save the sudoku solution as well
                savedSudoku.strSudokuSolution += iSudokuSolution(i, j).ToString
                savedSudoku.strSudokuSeeds += isSeed(i, j).ToString
            Next
        Next

        'Dim settings = IsolatedStorageSettings.ApplicationSettings
        Try
            If (appSettings.Contains(SAVED_SUDOKU)) Then
                appSettings.Remove(SAVED_SUDOKU)
            End If
            appSettings.Add(SAVED_SUDOKU, savedSudoku)
            'appSettings.Item(SAVED_SUDOKU) = savedSudoku

            appSettings.Save()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub btn0_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btn0.Click, btn1.Click, btn2.Click, btn3.Click, btn4.Click, btn5.Click, btn6.Click, btn7.Click, btn8.Click, btn9.Click
        Dim btnNumber As Button = sender

        If (Not tbIndexRow = 0) And (Not tbIndexCol = 0) Then
            Dim btnIndex As String = btnNumber.Name.Chars(btnNumber.Name.Length - 1)
            txtBox(tbIndexRow, tbIndexCol).BorderBrush = mySolidSysColorBrush

            Select Case btnIndex
                Case "1"
                    txtBox(tbIndexRow, tbIndexCol).Text = 1
                Case "2"
                    txtBox(tbIndexRow, tbIndexCol).Text = 2
                Case "3"
                    txtBox(tbIndexRow, tbIndexCol).Text = 3
                Case "4"
                    txtBox(tbIndexRow, tbIndexCol).Text = 4
                Case "5"
                    txtBox(tbIndexRow, tbIndexCol).Text = 5
                Case "6"
                    txtBox(tbIndexRow, tbIndexCol).Text = 6
                Case "7"
                    txtBox(tbIndexRow, tbIndexCol).Text = 7
                Case "8"
                    txtBox(tbIndexRow, tbIndexCol).Text = 8
                Case "9"
                    txtBox(tbIndexRow, tbIndexCol).Text = 9
                Case "0"
                    txtBox(tbIndexRow, tbIndexCol).Text = ""
            End Select
        End If
        Dim i As Integer = 0
        'For i = 0 To 9
        '    btnNum(i).IsEnabled = False
        'Next
        tbIndexRow = 0
        tbIndexCol = 0

        'Check if the puzzle is solved.
        Dim j As Integer = 0
        Dim incompleteFlg As Boolean = False
        For i = 1 To 9
            For j = 1 To 9
                Try
                    Dim x As Integer = Integer.Parse(txtBox(i, j).Text)
                    If (Not x = iSudokuSolution(i, j)) Then
                        incompleteFlg = True
                        Exit Sub
                    End If
                Catch ex As Exception
                    Exit Sub
                End Try
            Next
        Next
        If (Not incompleteFlg) Then
            MessageBox.Show("Congratulations! The puzzle is solved. Please rate the app if you like it!", "Winner", MessageBoxButton.OK)
            'puzzleInProgress = False
        End If
    End Sub

    Private Sub Hint_Click()
        MessageBox.Show("Coming Soon!", "Info", MessageBoxButton.OK)
    End Sub

    Private Sub btnCheck_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnCheck.Click
        Dim i, j As Integer
        Dim errorFlag As Boolean = False
        For i = 1 To 9
            For j = 1 To 9
                If (Not txtBox(i, j).Text = "") Then
                    If (Not Integer.Parse(txtBox(i, j).Text) = iSudokuSolution(i, j)) Then
                        errorFlag = True
                        Exit For
                    End If
                End If
            Next
            If (errorFlag) Then
                Exit For
            End If
        Next
        If (errorFlag) Then
            Dim mbResult As MessageBoxResult = MessageBox.Show("Mistake(s) exist. Do you want me to show?", "Warning", MessageBoxButton.OKCancel)
            If (mbResult = MessageBoxResult.OK) Then
                'MessageBox.Show("Error box is highlighted", "Error", MessageBoxButton.OK)
                txtBox(i, j).Focus()
            End If
        Else
            MessageBox.Show("So far so, good!", "Info", MessageBoxButton.OK)
        End If


    End Sub

    Private Sub SolutionItem_Click(sender As Object, e As EventArgs)
        Dim i, j As Integer
        If (puzzleInProgress) Then


            Dim mbResult As MessageBoxResult = MessageBox.Show("Are you sure, you want to see the solution?", "Warning", MessageBoxButton.OKCancel)
            If (Not mbResult = MessageBoxResult.OK) Then
                Exit Sub
            End If

            For i = 1 To 9
                For j = 1 To 9
                    txtBox(i, j).Text = iSudokuSolution(i, j).ToString
                Next
                btnNum(i).IsEnabled = False
            Next
            btn0.IsEnabled = False
            btnSave.IsEnabled = False
            btnCheck.IsEnabled = False
            puzzleInProgress = False
        End If
    End Sub

    Private Sub SettingsItem_Click(sender As Object, e As EventArgs)
        Me.NavigationService.Navigate(New Uri("/Settings.xaml", UriKind.Relative))
    End Sub
    Private Sub mySudokuSolver(sdk As Sudoku)
        'Find the solution for current sudoku, here it self..!!
        fillSudoku(sdk)
        sdk.solve()

    End Sub

End Class
