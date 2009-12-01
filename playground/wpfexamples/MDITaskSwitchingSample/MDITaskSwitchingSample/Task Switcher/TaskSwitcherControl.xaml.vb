Imports System.Collections.ObjectModel
Partial Public Class TaskSwitcherControl

#Region " Declarations "

    Private _objTabControl As TabControl
    Private _objTabItems As ObservableCollection(Of TabItemMetaData)

#End Region

#Region " Methods "

    Private Sub TaskSwitcherControl_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.PreviewKeyDown

        If e.Key = Key.Tab AndAlso e.KeyboardDevice.Modifiers = ModifierKeys.Control Then
            e.Handled = True

            If Me.lbFormList.Items.Count - 1 = Me.lbFormList.SelectedIndex Then
                Me.lbFormList.SelectedIndex = 0

            Else
                Me.lbFormList.SelectedIndex += 1
            End If

        End If

    End Sub

    Private Sub lbFormList_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lbFormList.SelectionChanged

        If Me.lbFormList Is Nothing OrElse Me.lbFormList.Items.Count = 0 OrElse Me.lbFormList.SelectedIndex = -1 Then
            Exit Sub
        End If

        Me.tbCurrentApplication.Text = CType(CType(_objTabControl.Items(Me.lbFormList.SelectedIndex), TabItem).Tag, TabItemMetaData).ApplicationName
        Me.rectFormPreview.Fill = New VisualBrush(CType(CType(_objTabControl.Items(Me.lbFormList.SelectedIndex), TabItem).Content, UIElement))

    End Sub

    Friend Sub Hide()
        _objTabControl.SelectedIndex = CType(CType(_objTabControl.Items(Me.lbFormList.SelectedIndex), TabItem).Tag, TabItemMetaData).TabControlIndex
        Me.Visibility = Windows.Visibility.Collapsed
        _objTabItems = Nothing
        Me.lbFormList.DataContext = Nothing

    End Sub

    Friend Sub Show(ByVal objTabControl As TabControl)
        _objTabControl = objTabControl

        If objTabControl.Items.Count < 1 Then
            Hide()
            Exit Sub
        End If

        _objTabItems = New ObservableCollection(Of TabItemMetaData)

        For intX As Integer = 0 To objTabControl.Items.Count - 1

            Dim obj As TabItem = objTabControl.Items(intX)
            Dim objTabItemMetaData As TabItemMetaData = CType(obj.Tag, TabItemMetaData)
            objTabItemMetaData.TabControlIndex = intX
            _objTabItems.Add(objTabItemMetaData)
        Next

        Me.lbFormList.DataContext = _objTabItems
        Me.UpdateLayout()
        Me.lbFormList.SelectedIndex = objTabControl.SelectedIndex

        Me.lbFormList.Focus()

    End Sub

#End Region

End Class
