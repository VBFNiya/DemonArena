'*************************************
'You may notice that a lot of derived DefinitionExpression classes
'have public fields instead of properties. This is deliberate. It is
'a performance enhancement. Reading fields are a lot faster than reading
'properties and DefinitionExpressions are accessed very very often.
'*************************************


Public MustInherit Class DefinitionExpression

End Class

Public Class Expr_KillThread
    Inherits DefinitionExpression
End Class

Public Class Expr_Frames
    Inherits DefinitionExpression

    Public SpriteName As String
    Public Frames As String
    Public Delay As Integer
    Public ExecuteInternalFunction As Action

    Public Sub New(ByVal spriteName As String, ByVal frames As String, ByVal delay As Integer)
        'Me.SpriteName = spriteName
        'Me.Delay = delay
        'Me.Frames = frames
        'Me.ExecuteInternalFunction = execute
        Me.New(spriteName, frames, delay, Nothing)
    End Sub


    Public Sub New(ByVal spriteName As String, ByVal frames As String, ByVal delay As Integer, ByVal execute As action)
        Me.SpriteName = spriteName
        Me.Delay = delay
        Me.Frames = frames
        Me.ExecuteInternalFunction = execute
    End Sub

End Class

Public Class Expr_Exec
    Inherits DefinitionExpression

    Public ExecuteDelegate As Action

    Public Sub New(ByVal execute As action)
        Me.ExecuteDelegate = execute
    End Sub


End Class

Public Class Expr_LoopBase
    Inherits DefinitionExpression

    Public Overridable ReadOnly Property NumberOfLoops As Integer
        Get
            Return Me.NumLoops
        End Get
    End Property

    Protected Property NumLoops As Integer
End Class

Public Class Expr_LoopRandom
    Inherits Expr_LoopBase

    Private _min, _max As Integer

    Public Sub New(ByVal minLoops As Integer, ByVal maxLoops As Integer)
        _min = minLoops
        _max = maxLoops
    End Sub

    Public ReadOnly Property MinLoops As Integer
        Get
            Return _min
        End Get
    End Property

    Public ReadOnly Property MaxLoops As Integer
        Get
            Return _max
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfLoops As Integer
        Get
            Dim rnd As New Random

            Return rnd.Next(_min, _max + 1)
        End Get
    End Property
End Class

Public Class Expr_LoopFixed
    Inherits Expr_LoopBase

    Public Sub New(ByVal numberOfLoops As Integer)
        Me.NumLoops = numberOfLoops
    End Sub
End Class

Public Class Expr_Label
    Inherits DefinitionExpression

    Public LabelName As String

    Public Sub New(ByVal labelName As String)
        Me.LabelName = labelName
    End Sub
End Class

Public Class Expr_Return
    Inherits DefinitionExpression
End Class

Public Class Expr_Goto
    Inherits DefinitionExpression

    Public GotoLabel As String

    Public Sub New(ByVal label As String)
        Me.GotoLabel = label
    End Sub

    Public Sub New(ByVal label As Expr_Label)
        Me.GotoLabel = label.LabelName
    End Sub
End Class

Public Class StandardStateLabels

    Private Shared _dying As New Expr_Label("Dying")
    Private Shared _thrusted As New Expr_Label("Thrusted")
    Private Shared _start As New Expr_Label("Start")
    Private Shared _hitArenaBoundry As New Expr_Label("HitBoundry")
    Private Shared _flinch As New Expr_Label("Flinch")

    Public Shared ReadOnly Property Flinch As Expr_Label
        Get
            Return _flinch
        End Get
    End Property

    Public Shared ReadOnly Property HitArenaEdge As Expr_Label
        Get
            Return _hitArenaBoundry
        End Get
    End Property


    Public Shared ReadOnly Property Start As Expr_Label
        Get
            Return _start
        End Get
    End Property


    Public Shared ReadOnly Property Thrusted As Expr_Label
        Get
            Return _thrusted
        End Get
    End Property

    Public Shared ReadOnly Property Dying As Expr_Label
        Get
            Return _dying
        End Get
    End Property
End Class