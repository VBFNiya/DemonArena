Public Enum HorizontalDirection
    None
    Left
    Right
End Enum
Public Enum VerticalDirection
    None
    Up
    Down
End Enum


Public Class MovementTrajectory
    Private _x As Double
    Private _y As Double

    'We store a speed value to allow
    'us to change the vector on a whim
    'yet preserve the current speed.
    'Private _speed As Double

    Public Shared Function CreateTrajectory(ByVal source As Point, ByVal target As Point, ByVal speed As Double) As MovementTrajectory
        Return CreateTrajectory(source.ToPointF, target.ToPointF, speed)
    End Function

    Public Shared Function CreateTrajectory(ByVal source As PointF, ByVal target As PointF, ByVal speed As Double) As MovementTrajectory
        Dim vec As New MovementTrajectory

        vec._x = target.X - source.X
        vec._y = target.Y - source.Y

        vec.AdjustSpeed(speed)

        Return vec
    End Function

    Public Sub New()

    End Sub

    Public Sub New(ByVal x As Double, ByVal y As Double)
        _x = x
        _y = y
    End Sub

    Public ReadOnly Property HorizontalDirection As HorizontalDirection
        Get
            If Me.X < 0 Then Return DemonArena.HorizontalDirection.Left
            If Me.X > 0 Then Return DemonArena.HorizontalDirection.Right

            Return DemonArena.HorizontalDirection.None
        End Get
    End Property

    Public ReadOnly Property VerticalDirection As VerticalDirection
        Get
            If Me.Y < 0 Then Return DemonArena.VerticalDirection.Up
            If Me.Y > 0 Then Return DemonArena.VerticalDirection.Down

            Return DemonArena.VerticalDirection.None
        End Get
    End Property

    Public ReadOnly Property Direction As SpriteDirection
        Get

            If Me.X = 0 AndAlso Me.Y > 0 Then
                Return SpriteDirection.Down
            End If

            If Me.X = 0 AndAlso Me.Y < 0 Then
                Return SpriteDirection.Up
            End If

            If Me.Y = 0 AndAlso Me.X > 0 Then
                Return SpriteDirection.Right
            End If

            If Me.Y = 0 AndAlso Me.X < 0 Then
                Return SpriteDirection.Left
            End If

            If Me.X > 0 AndAlso Me.Y > 0 Then
                Return SpriteDirection.LowerRight
            End If

            If Me.X > 0 AndAlso Me.Y < 0 Then
                Return SpriteDirection.UpperRight
            End If

            If Me.X < 0 AndAlso Me.Y > 0 Then
                Return SpriteDirection.LowerLeft
            End If

            If Me.X < 0 AndAlso Me.Y < 0 Then
                Return SpriteDirection.UpperLeft
            End If

            'If Me.X = 0 AndAlso Me.Y = 0 Then
            '    Return SpriteDirection.Down
            'End If

            Return SpriteDirection.Down

        End Get
    End Property

    Public ReadOnly Property X As Double
        Get
            Return _x
        End Get

    End Property

    Public ReadOnly Property Y As Double
        Get
            Return _y
        End Get
    End Property

    Public ReadOnly Property Integer_Y As Integer
        Get
            Return Math.Truncate(_y)
        End Get
    End Property
    Public ReadOnly Property Integer_X As Integer
        Get
            Return Math.Truncate(_x)
        End Get
    End Property
    Public ReadOnly Property Fraction_X As Double
        Get
            Return _x - Me.Integer_X
        End Get
    End Property
    Public ReadOnly Property Fraction_Y As Double
        Get
            Return _y - Me.Integer_Y
        End Get
    End Property

    Public Property Speed As Double
        Get
            'Dim xx As Double = Math.Abs(_x)
            'Dim yy As Double = Math.Abs(_y)

            'If xx > yy Then Return xx
            'If xx < yy Then Return yy

            ''We can reach here only when
            ''X and Y are equal hence
            ''we can return either one
            'Return xx

            Return Math.Sqrt(_x ^ 2 + _y ^ 2)
        End Get
        Set(ByVal value As Double)

            If value <= 0 Then value = 0

            AdjustSpeed(value)

            '_speed = value
        End Set
    End Property

    Public Function AsPointF() As PointF
        Return New PointF(_x, _y)
    End Function

    Public Function HasPassedPoint(ByVal currentPoint As PointF, ByVal targetPoint As PointF) As Boolean

        Dim passedX As Boolean
        Dim passedY As Boolean

        If _x < 0 Then
            If currentPoint.X < targetPoint.X Then passedX = True
        End If

        If _x > 0 Then
            If currentPoint.X > targetPoint.X Then passedX = True
        End If

        If _y < 0 Then
            If currentPoint.Y < targetPoint.Y Then passedY = True
        End If

        If _y > 0 Then
            If currentPoint.Y > targetPoint.Y Then passedY = True
        End If

        Return passedX AndAlso passedY
    End Function

    Public Sub SetHorizontalDirection(ByVal d As HorizontalDirection)

        If d = HorizontalDirection.Left Then
            _x = Neg(_x)
        End If

        If d = HorizontalDirection.Right Then
            _x = Math.Abs(_x)
        End If

    End Sub

    Public Sub SetVerticalDirection(ByVal d As VerticalDirection)
        If d = VerticalDirection.Down Then
            _y = Math.Abs(_y)
        End If

        If d = VerticalDirection.Up Then
            _y = Neg(_y)
        End If
    End Sub

    Public Sub SetTrajectoryMaintainSpeed(ByVal source As PointF, ByVal target As PointF)
        Me.SetTrajectory(source, target, Me.Speed)
    End Sub

    Public Sub SetTrajectoryMaintainSpeed(ByVal traj As PointF)
        Dim speed As Double = Me.Speed

        _x = traj.X
        _y = traj.Y

        AdjustSpeed(speed)
    End Sub

    Public Sub SetTrajectory(ByVal source As PointF, ByVal target As PointF, ByVal speed As Double)
        Me._x = target.X - source.X
        Me._y = target.Y - source.Y

        AdjustSpeed(speed)
    End Sub

    Private Sub AdjustSpeed(ByVal speed As Double)
        Dim pureTrajectory As PointDbl = GetPureTrajectory()

        _x = pureTrajectory.X * speed
        _y = pureTrajectory.Y * speed

    End Sub

    Private Function GetPureTrajectory() As PointDbl
        Dim radians As Double = Math.Atan2(_y, _x)

        Return New PointDbl(Math.Cos(radians), Math.Sin(radians))
    End Function

    'Use of this type ensures higher precision
    'when doing calculations.
    Private Class PointDbl
        Public Sub New(ByVal x As Double, ByVal y As Double)
            Me.X = x
            Me.Y = y
        End Sub
        Public Property X As Double
        Public Property Y As Double
    End Class


    'Private Sub AdjustSpeed(ByVal speed As Double)
    '    If speed <= 0 Then
    '        _x = 0
    '        _y = 0
    '        Return
    '    End If

    '    'The sign is important to determine
    '    'direction along the axis but we must
    '    'do speed calculations on absolute values
    '    'So use these variables to preserve the signs
    '    'after the calculations
    '    Dim bNegX As Boolean = _x < 0
    '    Dim bNegY As Boolean = _y < 0

    '    Dim absX As Double = Math.Abs(_x)
    '    Dim absY As Double = Math.Abs(_y)

    '    If absX <> 0 AndAlso absY <> 0 Then
    '        If absX > absY Then
    '            absY = (absY / absX) * speed
    '            absX = speed
    '        End If

    '        If absY > absX Then
    '            absX = (absX / absY) * speed
    '            absY = speed
    '        End If

    '        If absX = absY Then
    '            absY = speed
    '            absX = speed
    '        End If
    '    Else
    '        If absX = 0 Then
    '            absY = speed
    '        End If
    '        If absY = 0 Then
    '            absX = speed
    '        End If
    '    End If

    '    _x = absX
    '    _y = absY

    '    If bNegX Then _x = Neg(_x)
    '    If bNegY Then _y = Neg(_y)

    'End Sub

    Private Function Neg(ByVal number As Double) As Double
        If number = 0 Then Return 0

        If number < 0 Then
            Return number
        Else
            Return number - (number * 2)
        End If
    End Function
End Class

'Friend NotInheritable Class CreatureMover
'    Implements IDisposable

'    Private g_ptTargetPoint As Point
'    Private g_ctlControl As Control
'    'Private WithEvents g_tmrMove As New Timer
'    Private g_vecMovement As MovementVector

'    'We keep track of the closing distance
'    'As the form moves closer to the target
'    'the distance would naturally become smaller
'    'but if at any point that distance becomes larger
'    'then we know that we may have passed the target
'    'and we can stop there
'    Private g_dblSmallestDistance As Double

'    Private g_dblFraction_X As Double
'    Private g_dblFraction_Y As Double

'    Private g_dblInitSpeed As Double
'    Private g_dblDecelerationDistance As Double
'    Private g_bIsMoving As Boolean

'    Public Event MovementStarted As EventHandler
'    Public Event MovementCompleted As EventHandler

'    Public Property Decelerate As Boolean = True

'    Public ReadOnly Property IsMoving As Boolean
'        Get
'            Return g_bIsMoving
'        End Get
'    End Property

'    Public Property Target As Point
'        Get
'            Return g_ptTargetPoint
'        End Get
'        Set(ByVal value As Point)
'            g_ptTargetPoint = value
'        End Set
'    End Property

'    Public Property Control As Control
'        Get
'            Return g_ctlControl
'        End Get
'        Set(ByVal value As Control)
'            g_ctlControl = value
'        End Set
'    End Property

'    Public Sub [Stop]()
'        g_tmrMove.Stop()
'        RaiseEvent MovementCompleted(Me, New EventArgs)
'    End Sub


'    Public Sub Start(ByVal speed As Double)

'        g_vecMovement = MovementVector.CreateVector(Me.Control.Location, Me.Target, speed)

'        g_dblFraction_X = 0
'        g_dblFraction_Y = 0
'        g_dblInitSpeed = speed

'        'Deceleration would begin when 1/3 of the distance remains
'        g_dblDecelerationDistance = CalculateDistance(Me.Control.Location, Me.Target) / 3

'        'This value is constantly updated as the distance
'        'between the form and the target point closes
'        g_dblSmallestDistance = CalculateDistance(Me.Control.Location, Me.Target)

'        g_tmrMove.Interval = 1
'        g_tmrMove.Start()

'        RaiseEvent MovementStarted(Me, New EventArgs)

'    End Sub

'    Private Sub g_tmrMove_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles g_tmrMove.Tick
'        '************************************************************
'        'Note:
'        'Deceleration is achieved by rapidly decreasing the speed.
'        'The decrements are calculated using divisions so there
'        'are going to be remainers. So its obvious we
'        'have to use floating point types for to represent
'        'the movements on both axises. However, Control
'        'co-ordinates are Integers. They drop the fractions when 
'        'they are calculated with operations on floating point types.
'        'This results in the deceleration looking weird and unnatural.
'        'We correct this by continually adding up the fractions
'        'accumulated over multiple co-ordinate calculations and when
'        'those fractions add up to a mixed number, we do an
'        'extra co-ordinate calculation on that whole number and then 
'        'drop the whole number leaving the fraction to continue
'        'its accumulation. -- Niya
'        '*************************************************************

'        Dim curLoc As Point = Me.Control.Location

'        'See note above
'        g_dblFraction_X += g_vecMovement.Fraction_X
'        g_dblFraction_Y += g_vecMovement.Fraction_Y

'        If Math.Abs(g_dblFraction_X) >= 1 Then
'            'Add the integer part accumulated on the fractions
'            curLoc.X += Math.Truncate(g_dblFraction_X)

'            'Remove the integer and leave the fraction for
'            'more accumulation
'            g_dblFraction_X -= Math.Truncate(g_dblFraction_X)

'        End If

'        If Math.Abs(g_dblFraction_Y) >= 1 Then
'            'Add the integer part accumulated on the fractions
'            curLoc.Y += Math.Truncate(g_dblFraction_Y)

'            'Remove the integer and leave the fraction for
'            'more accumulation
'            g_dblFraction_Y -= Math.Truncate(g_dblFraction_Y)
'        End If

'        Dim dist As Double = CalculateDistance(curLoc, g_ptTargetPoint)

'        'If deceleration is set then we check to see if the we have
'        'our closed distance has become less than the deceleration distance
'        If Me.Decelerate AndAlso dist < g_dblDecelerationDistance Then

'            'Set the speed to represent the ratio of current distance to the
'            'distance at which the deceleration began.
'            'EG: If the deceleration distance is 100 and our current distance
'            'is 50 the the ratio is 50/100 which is half and that means
'            'we set our speed to half of our starting speed so if our starting speed
'            'was 30 it would become 15 here.
'            g_vecMovement.Speed = (dist / g_dblDecelerationDistance) * g_dblInitSpeed
'        End If

'        curLoc.X += g_vecMovement.Integer_X
'        curLoc.Y += g_vecMovement.Integer_Y

'        Me.Control.Location = curLoc

'        If dist > g_dblSmallestDistance OrElse dist = 0 Then
'            Me.Control.Location = g_ptTargetPoint
'            Me.Stop()
'        Else
'            g_dblSmallestDistance = dist
'        End If

'    End Sub

'    Private Function CalculateDistance(ByVal pt1 As Point, ByVal pt2 As Point) As Double
'        Return Math.Sqrt(((pt1.X - pt2.X) ^ 2) + ((pt1.Y - pt2.Y) ^ 2))
'    End Function

'#Region "IDisposable Support"
'    Private disposedValue As Boolean ' To detect redundant calls

'    ' IDisposable
'    Protected Sub Dispose(ByVal disposing As Boolean)
'        If Not Me.disposedValue Then
'            If disposing Then
'                g_tmrMove.Stop()
'                g_tmrMove.Dispose()
'            End If

'        End If
'        Me.disposedValue = True
'    End Sub

'    ' This code added by Visual Basic to correctly implement the disposable pattern.
'    Public Sub Dispose() Implements IDisposable.Dispose
'        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
'        Dispose(True)
'        GC.SuppressFinalize(Me)
'    End Sub
'#End Region

'End Class
