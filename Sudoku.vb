Public Class Sudoku
    'Public regn(3, 3) As region
    Public cel(9, 9) As Cell
    Public regn(3, 3) As region
    Private stk As New Stack(Of Cell)
    Public filledCount As Integer
    Sub New()
        Dim i, j As Integer

        For i = 1 To 9
            For j = 1 To 9
                cel(i, j) = New Cell
            Next j
        Next i

        For i = 1 To 3
            For j = 1 To 3
                regn(i, j) = New region
            Next j
        Next i
        filledCount = 0
    End Sub
    Public Sub getRow(ByVal cel As Cell)
        Dim j As Integer
        Dim s As String = ""
        For j = 1 To 9
            s = s & Me.cel(cel.RowId, j).value & " "
        Next j
        'MsgBox(s)
    End Sub
    Public Sub getColumn(ByVal cel As Cell)
        Dim i As Integer
        Dim s As String = ""
        For i = 1 To 9
            s = s & Me.cel(i, cel.ColId).value & " "
        Next i
        'MsgBox(s)
    End Sub
    Public Sub getRegion(ByVal cel As Cell)
        Dim i, j As Integer
        Dim s As String = ""
        For i = 1 To 3
            For j = 1 To 3
                s = s & Me.cel((regn(cel.rgnRowId, cel.rgncolId).cells(i, j).row), (regn(cel.rgnRowId, cel.rgncolId).cells(i, j).col)).value & " "
            Next j
        Next i
        'MsgBox(s)
    End Sub
    Public Function isInColumn(ByVal key As Integer, ByVal cel As Cell) As Boolean
        Dim i As Integer
        For i = 1 To 9
            If (key = Me.cel(i, cel.ColId).value) Then
                Return True
            End If
        Next i
        Return False
    End Function
    Public Function isTwiceInColumn(ByVal cel As Cell) As Boolean
        Dim i As Integer
        For i = 1 To 9
            If (cel.value = Me.cel(i, cel.ColId).value And i <> cel.RowId) Then
                Return True
            End If
        Next i
        Return False
    End Function
    Public Function isInRow(ByVal key As Integer, ByVal cel As Cell) As Boolean
        Dim j As Integer
        For j = 1 To 9
            If (key = Me.cel(cel.RowId, j).value) Then
                Return True
            End If
        Next j
        Return False
    End Function
    Public Function isTwiceInRow(ByVal cel As Cell) As Boolean
        Dim j As Integer
        For j = 1 To 9
            If (cel.value = Me.cel(cel.RowId, j).value And cel.ColId <> j) Then
                Return True
            End If
        Next j
        Return False
    End Function
    Public Function isInRgn(ByVal key As Integer, ByVal cel As Cell) As Boolean
        Dim i, j As Integer
        For i = 1 To 3
            For j = 1 To 3
                If (key = Me.cel((regn(cel.rgnRowId, cel.rgncolId).cells(i, j).row), (regn(cel.rgnRowId, cel.rgncolId).cells(i, j).col)).value) Then
                    Return True
                End If
            Next j
        Next i
        Return False
    End Function
    Public Function isTwiceInRegn(ByVal rgnRowId As Integer, ByVal rgnColId As Integer, ByVal cel As Cell) As Boolean
        Dim i, j As Integer
        For i = 1 To 3
            For j = 1 To 3
                'If (Me.regn(rgnRowId, rgnColId).cells(i, j) = cel.value) Then
                If ((cel.value = Me.cel((regn(rgnRowId, rgnColId).cells(i, j).row), (regn(rgnRowId, rgnColId).cells(i, j).col)).value) And
                    (cel.inRgnRowId <> i) And
                    (cel.inRgnColId <> j)) Then
                    Return True
                End If
            Next j
        Next i
        Return False
    End Function
    Public Sub solve()
        Dim oldFilledCount As Integer = 0
        While (filledCount <= 81) And (oldFilledCount <> filledCount)
            'MsgBox(filledCount)
            ' assign it before this changes
            oldFilledCount = filledCount
            fillByElimination()
        End While
        fillByBackTracking()
    End Sub
    Private Function fillByBackTracking() As String
        Dim i, j As Integer

        i = 1
        j = 1
        While (i <= 9)
            j = 1
            While (j <= 9)
                If (Not Me.cel(i, j).isFilled) Then
                    Dim pc As Integer
                    Dim c As Cell
                    Dim flag As Boolean = False

                    'As initial val is 0, make it one to start off the loop
                    If (cel(i, j).currentProbableValUsed = 0) Then
                        cel(i, j).currentProbableValUsed = 1
                    End If
                    c = cel(i, j)

                    For pc = c.currentProbableValUsed To c.countOfProbables
                        If (Not Me.isInRow(c.probableValues(pc), c)) And _
                           (Not Me.isInColumn(c.probableValues(pc), c)) And _
                           (Not Me.isInRgn(c.probableValues(pc), c)) Then

                            c.assignVal(c.probableValues(pc), pc)
                            Me.stk.Push(c)
                            flag = True
                            c.currentProbableValUsed = pc
                            j = j + 1
                            Exit For
                        End If
                    Next
                    If (flag = False) Then 'we could not place a val
                        'Reset the current probable val to 0 as it is being popped
                        c.value = 0
                        c.currentProbableValUsed = 0
                        Try
                            c = stk.Pop()
                        Catch ex As Exception
                            'MsgBox("Stack Problem..!! Sorry for the break.. I have to stop the application!! :-(( ")
                            'End
                            Return "Stack Problem..!! Sorry for the break.. I have to stop the application!! :-(( "
                        End Try
                        ' needed to start from the next probable value
                        c.isFilled = False
                        c.currentProbableValUsed = c.currentProbableValUsed + 1
                        i = c.RowId
                        j = c.ColId

                    End If
                Else
                    j = j + 1
                End If
            End While
            i = i + 1
        End While
        'print()
        Return ""
    End Function
    Private Sub fillByElimination()
        Dim i, j, keyVal As Integer
        Me.flushProbables()

        For i = 1 To 9
            For j = 1 To 9
                Dim p As Integer = 0
                For keyVal = 1 To 9
                    If (Me.cel(i, j).isFilled = False) Then

                        If (Me.isInColumn(keyVal, cel(i, j)) = False) And _
                        (Me.isInRow(keyVal, cel(i, j)) = False) And _
                        (Me.isInRgn(keyVal, cel(i, j)) = False) Then

                            p = p + 1
                            Me.cel(i, j).probableValues(p) = keyVal
                            Me.cel(i, j).countOfProbables = p
                        End If

                    End If
                Next keyVal
            Next
        Next

        For i = 1 To 9
            For j = 1 To 9
                If (cel(i, j).countOfProbables = 1) And (cel(i, j).isFilled = False) And (cel(i, j).isSeed = False) Then
                    'cel(i, j).value = cel(i, j).probableValues(1)
                    'cel(i, j).isFilled = True
                    cel(i, j).assignVal(cel(i, j).probableValues(1), True)
                    filledCount = filledCount + 1
                End If
            Next
        Next
        'Me.print()
    End Sub
    Public Sub fillRegions()
        Dim i, j, k, l As Integer

        Dim s As String = ""

        'For i = 1 To 3
        '    For j = 1 To 3
        '        For k = 1 To 3
        '            For l = 1 To 3
        '                For p = 1 To 9
        '                    For q = 1 To 9
        '                        If ((Me.cel(p, q).rgnRowId = i) And _
        '                            (Me.cel(p, q).rgncolId = j) And _
        '                            (Me.cel(p, q).inRgnRowId = k) And _
        '                            (Me.cel(p, q).inRgnColId = l)) Then _

        '                            Me.regn(i, j).cells(k, l).row = p
        '                            Me.regn(i, j).cells(k, l).col = q
        '                            Exit For
        '                        End If
        '                    Next q
        '                Next p
        '                s = s & regn(i, j).cells(k, l).row & "," & regn(i, j).cells(k, l).col & " "
        '            Next l
        '        Next k
        '        s = s & vbCrLf
        '    Next j
        'Next i
        'MsgBox(s)
        For i = 1 To 9
            For j = 1 To 9
                regn(cel(i, j).rgnRowId, cel(i, j).rgncolId).cells(cel(i, j).inRgnRowId, cel(i, j).inRgnColId).row = i
                regn(cel(i, j).rgnRowId, cel(i, j).rgncolId).cells(cel(i, j).inRgnRowId, cel(i, j).inRgnColId).col = j

            Next

        Next

        For i = 1 To 3
            For j = 1 To 3
                For k = 1 To 3
                    For l = 1 To 3
                        s = s & regn(i, j).cells(k, l).row & "," & regn(i, j).cells(k, l).col & " "
                    Next
                Next
                s = s & vbCrLf
            Next
        Next
        'MsgBox(s)

    End Sub

    Public Sub print()
        Dim i, j As Integer
        Dim str As String = ""
        For i = 1 To 9
            If (((i - 1) Mod 3 = 0) And ((i - 1) <> 0)) Then
                str = str & vbCrLf
            End If

            str = str & vbCrLf

            For j = 1 To 9
                If (((j - 1) Mod 3 = 0) And ((j - 1) <> 0)) Then
                    str = str & "    "
                End If

                str = str & Me.cel(i, j).value & "   "
            Next
        Next
        'MsgBox(str)
    End Sub
    Private Sub flushProbables()
        Dim i, j As Integer
        For i = 1 To 9
            For j = 1 To 9
                cel(i, j).countOfProbables = 0
            Next
        Next
    End Sub
    Public Sub flush()
        Me.filledCount = 0
        Me.stk.Clear()
        Dim i, j, k As Integer
        For i = 1 To 9
            For j = 1 To 9
                cel(i, j).value = 0
                cel(i, j).ColId = 0
                cel(i, j).RowId = 0
                cel(i, j).rgnRowId = 0
                cel(i, j).rgncolId = 0
                cel(i, j).inRgnRowId = 0
                cel(i, j).inRgnColId = 0
                cel(i, j).countOfProbables = 0
                cel(i, j).currentProbableValUsed = 0
                cel(i, j).isFilled = False
                cel(i, j).isSeed = False
                cel(i, j).probableValues(k) = 0
                System.Array.Clear(cel(i, j).probableValues, 1, 9)
            Next
        Next
    End Sub
End Class
