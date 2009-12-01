Partial Public Class PetMaintenanceControl
    Inherits UserControlBase

    Private Sub ContextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        MyBase.RaiseEvent_CloseParentContainer()
    End Sub
End Class
