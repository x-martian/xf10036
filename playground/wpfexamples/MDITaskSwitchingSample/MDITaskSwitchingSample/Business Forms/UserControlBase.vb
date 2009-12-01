
Public Class UserControlBase
    Inherits UserControl

#Region " Events "

    Public Shared ReadOnly CloseParentContainerRoutedEvent As RoutedEvent = EventManager.RegisterRoutedEvent("CloseParentContainer", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(UserControlBase))

    Public Custom Event CloseParentContainer As RoutedEventHandler

        AddHandler(ByVal value As RoutedEventHandler)
            Me.AddHandler(UserControlBase.CloseParentContainerRoutedEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As RoutedEventHandler)
            Me.RemoveHandler(UserControlBase.CloseParentContainerRoutedEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

#End Region

#Region " Methods "

    Protected Overridable Sub RaiseEvent_CloseParentContainer()

        RaiseEvent CloseParentContainer(Me, New RoutedEventArgs(CloseParentContainerRoutedEvent, Me))

    End Sub

#End Region

End Class
