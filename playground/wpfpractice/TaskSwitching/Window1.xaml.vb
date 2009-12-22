Class Window1 

    Private Sub MenuItem1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItem1.Click
        Close()
    End Sub

    Private Sub FileClose_CanExcute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        Debug.WriteLine("Window CanExcute")
        e.CanExecute = True
        e.Handled = True
    End Sub

    Private Sub FileClose_Excuted(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        userControl1.Visibility = Windows.Visibility.Hidden
        Me.dockPanel.Children.Remove(userControl1)
        Debug.WriteLine("Window Executed")
        Dim focused As Boolean = Menu1.Focus()
        Me.UpdateLayout()
    End Sub

    Private Sub FileOpen_CanExcute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = True
    End Sub

    Private Sub FileOpen_Excuted(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        userControl1.Visibility = Windows.Visibility.Visible
        Me.dockPanel.Children.Add(userControl1)
    End Sub

    Private Sub UserControl_CanExecute(ByVal sender As System.Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs)
        Debug.WriteLine("UserControl CanExecute")
    End Sub

    Private Sub UserControl_Executed(ByVal sender As System.Object, ByVal e As System.Windows.Input.ExecutedRoutedEventArgs)
        Debug.WriteLine("UserControl Executed")
    End Sub
End Class
