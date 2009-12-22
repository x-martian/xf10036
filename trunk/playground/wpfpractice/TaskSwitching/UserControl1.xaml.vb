Partial Public Class UserControl1
    Private Sub window1_LocationChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim pt As Point = New Point(0.0, 0.0)
        pt = Me.PointToScreen(pt)
        TextBox1.Text = pt.ToString()
    End Sub

    Private Sub UserControl1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        AddHandler Window.GetWindow(Me).LocationChanged, AddressOf window1_LocationChanged
    End Sub

    Private Sub UserControl1_IsVisibleChanged(ByVal sender As System.Object, ByVal e As System.Windows.DependencyPropertyChangedEventArgs) Handles MyBase.IsVisibleChanged
        If e.OldValue = False And e.NewValue = True Then
        ElseIf e.OldValue = True And e.NewValue = False Then
            RemoveHandler Window.GetWindow(Me).LocationChanged, AddressOf window1_LocationChanged
        End If
    End Sub
End Class
