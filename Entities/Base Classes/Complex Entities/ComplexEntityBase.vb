Public MustInherit Class ComplexEntityBase
    Implements ITickable

    Private g_lstDefinition As IList(Of DefinitionExpression)

    Private g_lstSprites As SpriteCollection
    Private g_objPrimaryExecutionUnit As DefinitionExecutionUnit
    Private g_lstSecondaryExecutionUnits As New List(Of DefinitionExecutionUnit)

    Private WithEvents g_objCurrentExecutionUnit As DefinitionExecutionUnit

    Private g_dblCurAlpha As Single

    Private _curRenderInfo As RenderInfo

    Private g_enCurDirection As SpriteDirection

    Public Sub New()
        g_dblCurAlpha = 1
    End Sub

    Public Sub Tick() Implements ITickable.Tick
        g_objCurrentExecutionUnit = g_objPrimaryExecutionUnit
        g_objPrimaryExecutionUnit.Tick()


        For Each eu In g_lstSecondaryExecutionUnits.ToArray
            g_objCurrentExecutionUnit = eu
            eu.Tick()


            If eu.ExecutionStopped Then
                g_lstSecondaryExecutionUnits.Remove(eu)
            End If
        Next

        'If frameUpdated Then
        '    If _curRenderInfo Is Nothing Then
        '        _curRenderInfo = New RenderInfo(Sprites.GetSprite(spriteName, g_enCurDirection, frame), g_dblCurAlpha)
        '    Else
        '        _curRenderInfo.Sprite = Sprites.GetSprite(spriteName, g_enCurDirection, frame)
        '        _curRenderInfo.Alpha = g_dblCurAlpha
        '    End If

        'End If



        OnTick()
    End Sub

    Protected Property Sprites As SpriteCollection

    Protected Property Alpha As Double
        Get
            Return g_dblCurAlpha
        End Get
        Set(value As Double)
            g_dblCurAlpha = value
            If g_dblCurAlpha < 0 Then g_dblCurAlpha = 0
        End Set
    End Property

    Protected ReadOnly Property CurrentRI As RenderInfo
        Get
            Return _curRenderInfo
        End Get
    End Property

    Protected Property ActorDefinition As IList(Of DefinitionExpression)
        Get
            Return g_lstDefinition
        End Get
        Set(ByVal value As IList(Of DefinitionExpression))
            g_lstDefinition = value

            If g_lstDefinition IsNot Nothing Then
                g_objPrimaryExecutionUnit = New DefinitionExecutionUnit(g_lstDefinition)
                g_objCurrentExecutionUnit = g_objPrimaryExecutionUnit
            End If
        End Set
    End Property

    Protected MustOverride Sub OnTick()

    Protected Sub ExecuteStateParallel(ByVal state As String)
        If Me.HasStateInEntireDef(state) Then
            'Create a new execution unit so we can execute
            'the state in parallel.
            Dim exeUnit As New DefinitionExecutionUnit(g_lstDefinition)

            'Jump to the state within that execution unit
            'We do not push a return pointer because it is not a typical
            'goto call. It would be like jumping threads 
            exeUnit.GotoState(state, True, False)

            'Start executing
            g_lstSecondaryExecutionUnits.Add(exeUnit)
        End If
    End Sub

    Protected Sub Yield(ByVal cycles As Integer)
        g_objCurrentExecutionUnit.SetDelay(cycles)
    End Sub

    '**** I may have use for something like this
    'Protected Sub ExecuteStateParallel(ByVal state As String)
    '    Dim copy As Boolean = False
    '    Dim lstState As New List(Of DefinitionExpression)(100)

    '    If Me.HasStateInEntireDef(state) Then

    '        'Cycle the list of expressions
    '        For Each expr As DefinitionExpression In g_lstDefinition

    '            'If True means we are currently inside the state we're looking for
    '            If copy Then
    '                lstState.Add(expr)
    '            End If

    '            'Single out label expressions
    '            If TypeOf expr Is Expr_Label Then

    '                'If we aren't copying then that means we
    '                'are still looking for the label
    '                If Not copy Then
    '                    'Cast it
    '                    Dim exLabel As Expr_Label = DirectCast(expr, Expr_Label)

    '                    'Is it the label we're looking for ?
    '                    If exLabel.LabelName.Equals(state, StringComparison.CurrentCultureIgnoreCase) Then
    '                        'We start copying from here on out
    '                        lstState.Add(expr)
    '                        copy = True
    '                    End If
    '                Else
    '                    'We have reached another label after the one we were looking for
    '                    'so we stop looping as we have now copied the whole state
    '                    'definition
    '                    Exit For
    '                End If
    '            End If
    '        Next
    '    End If

    '    If lstState.Count > 0 Then
    '        'Create a new execution unit
    '        Dim exeUnit As New DefinitionExecutionUnit(lstState)

    '        'Start executing it.
    '        g_lstSecondaryExecutionUnits.Add(exeUnit)
    '    End If


    'End Sub

    Protected Function CurrentStateIs(ByVal state As String) As Boolean
        Return g_objCurrentExecutionUnit.CurrentStateIs(state)
    End Function

    Protected Function HasTimer(ByVal handlerState As String) As Boolean
        Return g_objCurrentExecutionUnit.HasTimer(handlerState)
    End Function

    Protected Sub CreateTimer(ByVal interval As Integer, ByVal handlerState As String, ByVal fireOnce As Boolean)
        g_objCurrentExecutionUnit.CreateTimer(interval, handlerState, fireOnce)
    End Sub

    Protected Sub RemoveTimer(ByVal handlerState As String)
        g_objCurrentExecutionUnit.RemoveTimer(handlerState)
    End Sub

    Protected Sub GotoState(ByVal stateLabel As String)
        g_objCurrentExecutionUnit.GotoState(stateLabel)
    End Sub

    Protected Sub SetDirection(ByVal d As SpriteDirection)
        g_enCurDirection = d
    End Sub

    Protected Function HasState(ByVal state As String) As Boolean
        Return g_objCurrentExecutionUnit.HasState(state)
    End Function

    'Use if a derived class wants to know when the rendering information
    'for this entity has changed. 
    Protected Overridable Sub OnRenderInfoChanged()

    End Sub

    Private Function HasStateInEntireDef(ByVal state As String) As Boolean
        For Each def As Expr_Label In g_lstDefinition.OfType(Of Expr_Label)()
            If def.LabelName.Equals(state, StringComparison.CurrentCultureIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    'Private Sub g_objCurrentExecutionUnit_ExecutionTerminationRequested(sender As Object, e As System.EventArgs) Handles g_objCurrentExecutionUnit.ExecutionTerminationRequested


    'End Sub

    Private Sub ExecutionUnit_FrameChanged(sender As Object, e As FrameChangedEventArgs) Handles g_objCurrentExecutionUnit.FrameChanged
        If _curRenderInfo Is Nothing Then
            _curRenderInfo = New RenderInfo(Sprites.GetSprite(e.SpriteName, g_enCurDirection, e.Frame), g_dblCurAlpha)
        Else
            _curRenderInfo.Sprite = Sprites.GetSprite(e.SpriteName, g_enCurDirection, e.Frame)
            _curRenderInfo.Alpha = g_dblCurAlpha
        End If

        OnRenderInfoChanged()
    End Sub
End Class


Friend Class DefinitionExecutionUnit
    Private g_bStopExecution As Boolean
    Private g_iDelayCountDown As Integer
    Private g_dlgExecuteNext As Action
    Private g_iCurrentExprIndex As Integer
    Private g_stkReturnIndexes As New Stack(Of Integer)
    Private g_lstTimers As New List(Of TickTimer)

    'Execution units take up a lot of the executing time
    'so we optimize as best we could. This was of dealing with EventArgs
    'avoids us having to instantiate a new one every time a frame changes
    Private g_objFrameChangedEA As New FrameChangedEventArgs

    'A flattened list of the creature's definition
    Private g_lstDefFlattened As IList(Of DefinitionExpression)
    Private g_sCurrentStateName As String

    Public Event ExecutionTerminationRequested As EventHandler
    Protected Sub OnExecutionTerminationRequested()
        RaiseEvent ExecutionTerminationRequested(Me, EventArgs.Empty)
    End Sub

    Public Event FrameChanged As EventHandler(Of FrameChangedEventArgs)
    Protected Sub OnFrameChanged(ByVal e As FrameChangedEventArgs)
        RaiseEvent FrameChanged(Me, e)
    End Sub

    Public Sub New()

    End Sub

    Public Sub New(ByVal definition As IEnumerable(Of DefinitionExpression))
        Me.SetDefinition(definition)
    End Sub

    Public ReadOnly Property ExecutionStopped As Boolean
        Get
            Return g_bStopExecution
        End Get
    End Property

    Public ReadOnly Property CurrentState As String
        Get
            Return g_sCurrentStateName
        End Get
    End Property

    Public Sub SetDelay(ByVal delay As Integer)
        g_iDelayCountDown = delay
    End Sub

    Public Sub SetDefinition(ByVal def As IEnumerable(Of DefinitionExpression))
        g_lstDefFlattened = FlattenDefinition(def)
    End Sub

    Public Sub Tick()
        If g_bStopExecution Then
            Exit Sub
        End If

        'Me.FrameUpdated = False

        If g_iDelayCountDown < 1 Then

            Do Until g_iDelayCountDown > 0

                If g_dlgExecuteNext IsNot Nothing Then
                    g_dlgExecuteNext.Invoke()
                    g_dlgExecuteNext = Nothing
                End If

                Dim expr = g_lstDefFlattened.Item(g_iCurrentExprIndex)

                If TypeOf expr Is Expr_KillThread Then
                    OnExecutionTerminationRequested()
                    g_bStopExecution = True
                    Exit Do
                End If

                If TypeOf expr Is Expr_Return Then
                    Dim casted As Expr_Return =
                        DirectCast(expr, Expr_Return)

                    If g_stkReturnIndexes.Count > 0 Then
                        InternalGotoLine(g_stkReturnIndexes.Pop, True)
                    End If
                End If

                If TypeOf expr Is Expr_Label Then
                    Dim casted As Expr_Label = DirectCast(expr, Expr_Label)

                    If g_sCurrentStateName = "" Then


                        g_sCurrentStateName = casted.LabelName
                    Else 'We have strayed into another state

                        'This ensures that we loop
                        'states implicitly by going back to the start of
                        'the current state
                        GotoState(g_sCurrentStateName, True, False)
                    End If
                End If

                If TypeOf expr Is Expr_LoopCountDownBase Then
                    Dim lexpr As Expr_LoopCountDownBase =
                        DirectCast(expr, Expr_LoopCountDownBase)

                    If lexpr.CurrentLoopCount = lexpr.NumberOfLoops - 1 Then

                        'Reset the loop expression
                        ResetLoopExpressionIfNeeded(g_iCurrentExprIndex)
                    Else
                        lexpr.CurrentLoopCount += 1
                        GotoState(g_sCurrentStateName, False, False)
                    End If
                End If

                If TypeOf expr Is Expr_LoopBase Then
                    Dim lexp As Expr_LoopBase = DirectCast(expr, Expr_LoopBase)

                    Dim tempLoopExpr As Expr_LoopCountDownBase = Nothing

                    If TypeOf lexp Is Expr_LoopFixed Then
                        tempLoopExpr = New Expr_LoopCountDownFixed(lexp.NumberOfLoops)
                    End If

                    If TypeOf lexp Is Expr_LoopRandom Then
                        Dim t As Expr_LoopRandom = DirectCast(lexp, Expr_LoopRandom)
                        tempLoopExpr = New Expr_LoopCountDownRandom(t.NumberOfLoops, t.MinLoops, t.MaxLoops)
                    End If

                    'Replace the loop expression with
                    'a temporary expression that keeps a count
                    'of the number of times the loop has ran
                    g_lstDefFlattened.Item(g_iCurrentExprIndex) = tempLoopExpr

                    'Go back so the temporary loop expression can be processed
                    g_iCurrentExprIndex -= 1

                End If

                If TypeOf expr Is Expr_Goto Then
                    GotoState(DirectCast(expr, Expr_Goto).GotoLabel)
                End If

                If TypeOf expr Is Expr_Exec Then
                    Dim cmd As Expr_Exec = DirectCast(expr, Expr_Exec)
                    cmd.ExecuteDelegate.Invoke()
                End If

                If TypeOf expr Is Expr_Frames Then
                    Dim fe As Expr_Frames = DirectCast(expr, Expr_Frames)
                    '_curRenderInfo = New RenderInfo(Sprites.GetSprite(fe.SpriteName, g_enCurDirection, fe.Frames(0)), g_dblCurAlpha)

                    g_objFrameChangedEA.SpriteName = fe.SpriteName
                    g_objFrameChangedEA.Frame = fe.Frames(0)
                    OnFrameChanged(g_objFrameChangedEA)

                    'OnFrameChanged(New FrameChangedEventArgs(fe.SpriteName, fe.Frames(0)))

                    'Me.FrameUpdated = True
                    'Me.FrameLetter = fe.Frames(0)
                    'Me.SpriteName = fe.SpriteName

                    g_iDelayCountDown = fe.Delay
                    g_dlgExecuteNext = fe.ExecuteInternalFunction
                End If

                'The reason we fire timers in here and not outside
                'is to keep them synchronized with the execution
                'of expressions in an actor's definition.
                'We don't want timers jumping in the middle of
                'a delay. This should make returning from
                'timer jumps smooth
                For Each tmr As TickTimer In g_lstTimers.ToArray
                    If tmr.CountDown = 0 Then
                        If HasState(tmr.HandlerState) Then
                            GotoState(tmr.HandlerState)
                        End If

                        If tmr.FireOnce Then
                            g_lstTimers.Remove(tmr)
                        Else
                            tmr.CountDown = tmr.Interval
                        End If
                    End If
                Next

                g_iCurrentExprIndex += 1

                If g_iCurrentExprIndex = g_lstDefFlattened.Count Then
                    GotoState(g_sCurrentStateName)
                End If
            Loop

        Else
            g_iDelayCountDown -= 1
        End If

        For Each tmr In g_lstTimers

            'Pause count down as long as we're in the state that this
            'timer would handle
            If tmr.CountDown > 0 AndAlso Not g_sCurrentStateName.Equals(tmr.HandlerState, StringComparison.CurrentCultureIgnoreCase) Then
                tmr.CountDown -= 1
            End If
        Next
    End Sub

    Public Function CurrentStateIs(ByVal state As String) As Boolean
        Return state.Equals(Me.CurrentState, StringComparison.CurrentCultureIgnoreCase)
    End Function

    Public Function HasTimer(ByVal handlerState As String) As Boolean
        Dim tmr As TickTimer = g_lstTimers.FirstOrDefault(Function(t) t.HandlerState.Equals(handlerState, StringComparison.CurrentCultureIgnoreCase))

        Return tmr IsNot Nothing
    End Function

    Public Sub CreateTimer(ByVal interval As Integer, ByVal handlerState As String, ByVal fireOnce As Boolean)
        g_lstTimers.Add(New TickTimer(handlerState, interval, fireOnce))
    End Sub

    Public Sub RemoveTimer(ByVal handlerState As String)
        Dim tmr = g_lstTimers.FirstOrDefault(Function(t) t.HandlerState.Equals(handlerState, StringComparison.CurrentCultureIgnoreCase))

        If tmr IsNot Nothing Then
            g_lstTimers.Remove(tmr)
        End If

    End Sub

    Public Function HasState(ByVal state As String) As Boolean
        For Each def As Expr_Label In g_lstDefFlattened.OfType(Of Expr_Label)()
            If def.LabelName.Equals(state, StringComparison.CurrentCultureIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function GotoState(ByVal state As String) As Boolean
        Return GotoState(state, True, True)
    End Function

    Public Function GotoState(ByVal state As String, ByVal resetLoops As Boolean, ByVal pushReturn As Boolean) As Boolean
        Dim jumpSuccessful As Boolean = False

        For i = 0 To g_lstDefFlattened.Count - 1
            If resetLoops Then
                ResetLoopExpressionIfNeeded(i)
            End If

            Dim def As Expr_Label = TryCast(g_lstDefFlattened.Item(i), Expr_Label)

            If def IsNot Nothing AndAlso Not jumpSuccessful Then
                If def.LabelName.Equals(state, StringComparison.CurrentCultureIgnoreCase) Then

                    'This is used to implement looping
                    'which is technically a Goto but is semantically,
                    'a loop. We can return from Gotos but not a Goto
                    'used to implement a loop
                    If pushReturn Then
                        'So "Return" expression can return to this
                        'point in the state execution
                        g_stkReturnIndexes.Push(g_iCurrentExprIndex)
                    End If

                    g_iCurrentExprIndex = i
                    g_sCurrentStateName = def.LabelName

                    jumpSuccessful = True

                    'We allow iteration to continue
                    'to reset all loop definitions
                    If Not resetLoops Then
                        Exit For
                    End If
                End If
            End If
        Next
        Return jumpSuccessful
    End Function


    'Splits frame expression lines where multiple frames
    'are expressed in a single line into multiple single frame
    'expressions
    Private Function FlattenDefinition(ByVal definition As IEnumerable(Of DefinitionExpression)) As DefinitionExpression()
        Dim lstNewDef As New List(Of DefinitionExpression)

        For Each de In definition
            If TypeOf de Is Expr_Frames Then
                Dim fe As Expr_Frames = DirectCast(de, Expr_Frames)

                If fe.Frames.Length > 1 Then
                    For Each frame In fe.Frames
                        lstNewDef.Add(New Expr_Frames(fe.SpriteName, frame.ToString, fe.Delay, Nothing))
                    Next

                    'Put the function call on the last frame
                    If fe.ExecuteInternalFunction IsNot Nothing Then
                        DirectCast(lstNewDef.Last, Expr_Frames).ExecuteInternalFunction = fe.ExecuteInternalFunction
                    End If
                Else 'Expression is already a single frame expression


                    lstNewDef.Add(fe)
                End If
            Else
                lstNewDef.Add(de)
            End If

        Next

        Return lstNewDef.ToArray
    End Function

    Private Function InternalGotoLine(ByVal index As Integer, ByVal resetLoops As Boolean) As Boolean
        Dim curState As String = ""
        Dim jumpSuccessful As Boolean = False

        For i = 0 To g_lstDefFlattened.Count - 1
            If resetLoops Then ResetLoopExpressionIfNeeded(i)

            Dim exp As DefinitionExpression = g_lstDefFlattened.Item(i)

            If TypeOf exp Is Expr_Label Then
                curState = DirectCast(exp, Expr_Label).LabelName
            End If

            If i = index Then
                g_iCurrentExprIndex = i
                g_sCurrentStateName = curState
                jumpSuccessful = True

                If Not resetLoops Then
                    Exit For
                End If

            End If
        Next
        Return jumpSuccessful
    End Function



    Private Sub ResetLoopExpressionIfNeeded(ByVal index As Integer)

        Dim def As DefinitionExpression = g_lstDefFlattened.Item(index)

        If TypeOf def Is Expr_LoopCountDownFixed Then
            'Temporary loop expression
            Dim tempExpr = DirectCast(def, Expr_LoopCountDownFixed)

            'Oringinal loop expression recreated
            Dim originalExp As New Expr_LoopFixed(tempExpr.NumberOfLoops)

            g_lstDefFlattened.Item(index) = originalExp
        End If

        If TypeOf def Is Expr_LoopCountDownRandom Then
            'Temporary loop expression
            Dim tempExpr = DirectCast(def, Expr_LoopCountDownRandom)

            'Oringinal loop expression recreated
            Dim originalExp As New Expr_LoopRandom(tempExpr.MinLoops, tempExpr.MaxLoops)

            g_lstDefFlattened.Item(index) = originalExp

        End If

    End Sub


    Private Class TickTimer
        Public Sub New(ByVal handlerState As String, ByVal interval As Integer, ByVal fireOnce As Boolean)
            Me.FireOnce = fireOnce
            Me.CountDown = interval
            Me.Interval = interval
            Me.HandlerState = handlerState
        End Sub

        Public Property Interval As Integer

        'The state that it jumps to once
        'the timer has fired
        Public Property HandlerState As String
        Public Property FireOnce As Boolean
        Public Property CountDown As Integer
    End Class

    Private Class Expr_LoopCountDownBase
        Inherits DefinitionExpression

        Private _numLoops As Integer
        Public Sub New(ByVal numberOfLoops As Integer)
            Me.CurrentLoopCount = 0
            _numLoops = numberOfLoops
        End Sub

        Public ReadOnly Property NumberOfLoops As Integer
            Get
                Return _numLoops
            End Get
        End Property
        Public Property CurrentLoopCount As Integer
    End Class

    Private Class Expr_LoopCountDownRandom
        Inherits Expr_LoopCountDownBase

        Private _min As Integer
        Private _max As Integer

        Public Sub New(ByVal numLoops As Integer, ByVal minLoops As Integer, ByVal maxLoops As Integer)
            MyBase.New(numLoops)
            _max = maxLoops
            _min = minLoops
        End Sub

        Public ReadOnly Property MaxLoops As Integer
            Get
                Return _max
            End Get
        End Property

        Public ReadOnly Property MinLoops As Integer
            Get
                Return _min
            End Get
        End Property

    End Class

    Private Class Expr_LoopCountDownFixed
        Inherits Expr_LoopCountDownBase

        Public Sub New(ByVal numberOfLoops As Integer)
            MyBase.new(numberOfLoops)
        End Sub
    End Class

    Private Class ReturnPointer
        Public Sub New(ByVal state As String, ByVal index As Integer)
            Me.State = state
            Me.Index = index
        End Sub

        Public Property State As String

        'Index of the definition expression
        Public Property Index As Integer

    End Class



End Class

Public Class FrameChangedEventArgs
    Inherits EventArgs

    Public SpriteName As String
    Public Frame As Char

    'Private _sprName As String
    'Private _frame As Char

    'Public Sub New(ByVal spriteName As String, ByVal frame As Char)
    '    '   _sprName = spriteName
    '    '   _frame = frame
    'End Sub



    'Public ReadOnly Property SpriteName As String
    '    Get
    '        Return _sprName
    '    End Get
    'End Property

    'Public ReadOnly Property Frame As Char
    '    Get
    '        Return _frame
    '    End Get
    'End Property
End Class

Public MustInherit Class ComplexEntityBaseOLD
    Implements ITickable

    Private g_lstDefinition As IList(Of DefinitionExpression)
    Private g_lstDefFlattened As IList(Of DefinitionExpression)
    Private g_lstSprites As SpriteCollection
    Private g_stkReturnIndexes As New Stack(Of Integer)
    Private g_lstTimers As New List(Of TickTimer)

    Private g_iCurrentExprIndex As Integer
    Private g_iDelayCountDown As Integer
    Private g_sCurrentStateName As String
    Private g_dblCurAlpha As Single

    Private _curRenderInfo As RenderInfo

    Private g_enCurDirection As SpriteDirection
    Private g_dlgExecuteNext As [Delegate]
    Private g_iLoopCountDown As Integer

    Public Sub New()
        g_iCurrentExprIndex = 0
        g_iDelayCountDown = 0
        g_dblCurAlpha = 1
        g_dlgExecuteNext = Nothing
        g_sCurrentStateName = ""
        g_iLoopCountDown = -1
    End Sub

    Public Sub Tick() Implements ITickable.Tick
        If g_iDelayCountDown < 1 Then

            Do Until g_iDelayCountDown > 0

                If g_dlgExecuteNext IsNot Nothing Then
                    g_dlgExecuteNext.DynamicInvoke()
                    g_dlgExecuteNext = Nothing
                End If

                Dim expr = g_lstDefFlattened.Item(g_iCurrentExprIndex)

                If TypeOf expr Is Expr_Return Then
                    Dim casted As Expr_Return =
                        DirectCast(expr, Expr_Return)

                    If g_stkReturnIndexes.Count > 0 Then
                        InternalGotoLine(g_stkReturnIndexes.Pop, True)
                    End If
                End If

                If TypeOf expr Is Expr_Label Then
                    Dim casted As Expr_Label = DirectCast(expr, Expr_Label)

                    If g_sCurrentStateName = "" Then


                        g_sCurrentStateName = casted.LabelName
                    Else 'We have strayed into another state

                        'This ensures that we loop
                        'states implicitly by going back to the start of
                        'the current state
                        InternalGotoState(g_sCurrentStateName, True, False)
                    End If
                End If

                If TypeOf expr Is Expr_LoopCountDownBase Then
                    Dim lexpr As Expr_LoopCountDownBase =
                        DirectCast(expr, Expr_LoopCountDownBase)

                    If lexpr.CurrentLoopCount = lexpr.NumberOfLoops - 1 Then

                        'Reset the loop expression
                        ResetLoopExpressionIfNeeded(g_iCurrentExprIndex)
                    Else
                        lexpr.CurrentLoopCount += 1
                        InternalGotoState(g_sCurrentStateName, False, False)
                    End If
                End If

                If TypeOf expr Is Expr_LoopBase Then
                    Dim lexp As Expr_LoopBase = DirectCast(expr, Expr_LoopBase)

                    Dim tempLoopExpr As Expr_LoopCountDownBase = Nothing

                    If TypeOf lexp Is Expr_LoopFixed Then
                        tempLoopExpr = New Expr_LoopCountDownFixed(lexp.NumberOfLoops)
                    End If

                    If TypeOf lexp Is Expr_LoopRandom Then
                        Dim t As Expr_LoopRandom = DirectCast(lexp, Expr_LoopRandom)
                        tempLoopExpr = New Expr_LoopCountDownRandom(t.NumberOfLoops, t.MinLoops, t.MaxLoops)
                    End If

                    'Replace the loop expression with
                    'a temporary expression that keeps a count
                    'of the number of times the loop has ran
                    g_lstDefFlattened.Item(g_iCurrentExprIndex) = tempLoopExpr

                    'Go back so the temporary loop expression can be processed
                    g_iCurrentExprIndex -= 1

                End If

                If TypeOf expr Is Expr_Goto Then
                    InternalGotoState(DirectCast(expr, Expr_Goto).GotoLabel)
                End If

                If TypeOf expr Is Expr_Exec Then
                    Dim cmd As Expr_Exec = DirectCast(expr, Expr_Exec)
                    cmd.ExecuteDelegate.DynamicInvoke()

                End If

                If TypeOf expr Is Expr_Frames Then
                    Dim fe As Expr_Frames = DirectCast(expr, Expr_Frames)
                    _curRenderInfo = New RenderInfo(Sprites.GetSprite(fe.SpriteName, g_enCurDirection, fe.Frames(0)), g_dblCurAlpha)

                    g_iDelayCountDown = fe.Delay
                    g_dlgExecuteNext = fe.ExecuteInternalFunction

                End If

                'The reason we fire timers in here and not outside
                'is to keep them synchronized with the execution
                'of expressions in an actor's definition.
                'We don't want timers jumping in the middle of
                'a delay. This should make returning from
                'timer jumps smooth
                For Each tmr As TickTimer In g_lstTimers.ToArray
                    If tmr.CountDown = 0 Then
                        If HasState(tmr.HandlerState) Then
                            InternalGotoState(tmr.HandlerState)
                        End If

                        If tmr.FireOnce Then
                            g_lstTimers.Remove(tmr)
                        Else
                            tmr.CountDown = tmr.Interval
                        End If
                    End If
                Next

                g_iCurrentExprIndex += 1

                If g_iCurrentExprIndex = g_lstDefFlattened.Count Then
                    InternalGotoState(g_sCurrentStateName)
                End If
            Loop

        Else
            g_iDelayCountDown -= 1
        End If


        For Each tmr In g_lstTimers

            'Pause count down as long as we're in the state that this
            'timer would handle
            If tmr.CountDown > 0 AndAlso Not g_sCurrentStateName.Equals(tmr.HandlerState, StringComparison.CurrentCultureIgnoreCase) Then
                tmr.CountDown -= 1
            End If
        Next

        OnTick()
    End Sub

    Protected Property Sprites As SpriteCollection

    Protected ReadOnly Property CurrentState As String
        Get
            Return g_sCurrentStateName
        End Get
    End Property

    Protected ReadOnly Property CurrentRI As RenderInfo
        Get
            Return _curRenderInfo
        End Get
    End Property

    Protected Property ActorDefinition As IList(Of DefinitionExpression)
        Get
            Return g_lstDefinition
        End Get
        Set(ByVal value As IList(Of DefinitionExpression))
            g_lstDefinition = value

            If g_lstDefinition IsNot Nothing Then
                g_lstDefFlattened = FlattenDefinition(g_lstDefinition)
            End If
        End Set
    End Property

    Protected MustOverride Sub OnTick()

    Protected Function CurrentStateIs(ByVal state As String) As Boolean
        Return state.Equals(Me.CurrentState, StringComparison.CurrentCultureIgnoreCase)
    End Function

    Protected Function HasTimer(ByVal handlerState As String) As Boolean
        Dim tmr As TickTimer = g_lstTimers.FirstOrDefault(Function(t) t.HandlerState.Equals(handlerState, StringComparison.CurrentCultureIgnoreCase))

        Return tmr IsNot Nothing
    End Function

    Protected Sub CreateTimer(ByVal interval As Integer, ByVal handlerState As String, ByVal fireOnce As Boolean)
        g_lstTimers.Add(New TickTimer(handlerState, interval, fireOnce))
    End Sub

    Protected Sub RemoveTimer(ByVal handlerState As String)
        Dim tmr = g_lstTimers.FirstOrDefault(Function(t) t.HandlerState.Equals(handlerState, StringComparison.CurrentCultureIgnoreCase))

        If tmr IsNot Nothing Then
            g_lstTimers.Remove(tmr)
        End If

    End Sub

    Protected Sub GotoState(ByVal stateLabel As String)
        InternalGotoState(stateLabel)
    End Sub

    Protected Sub SetDirection(ByVal d As SpriteDirection)
        g_enCurDirection = d
    End Sub

    Protected Function HasState(ByVal state As String) As Boolean
        For Each def As Expr_Label In g_lstDefFlattened.OfType(Of Expr_Label)()
            If def.LabelName.Equals(state, StringComparison.CurrentCultureIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function InternalGotoState(ByVal state As String) As Boolean
        Return InternalGotoState(state, True, True)
    End Function

    Private Function InternalGotoState(ByVal state As String, ByVal resetLoops As Boolean, ByVal pushReturn As Boolean) As Boolean
        Dim jumpSuccessful As Boolean = False

        For i = 0 To g_lstDefFlattened.Count - 1
            If resetLoops Then
                ResetLoopExpressionIfNeeded(i)
            End If

            Dim def As Expr_Label = TryCast(g_lstDefFlattened.Item(i), Expr_Label)

            If def IsNot Nothing AndAlso Not jumpSuccessful Then
                If def.LabelName.Equals(state, StringComparison.CurrentCultureIgnoreCase) Then

                    'This is used to implement looping
                    'which is technically a Goto but is semantically,
                    'a loop. We can return from Gotos but not a Goto
                    'used to implement a loop
                    If pushReturn Then
                        'So "Return" expression can return to this
                        'point in the state execution
                        g_stkReturnIndexes.Push(g_iCurrentExprIndex)
                    End If

                    g_iCurrentExprIndex = i
                    g_sCurrentStateName = def.LabelName

                    jumpSuccessful = True

                    'We allow iteration to continue
                    'to reset all loop definitions
                    If Not resetLoops Then
                        Exit For
                    End If
                End If
            End If
        Next
        Return jumpSuccessful
    End Function

    Private Sub ResetLoopExpressionIfNeeded(ByVal index As Integer)

        Dim def As DefinitionExpression = g_lstDefFlattened.Item(index)

        If TypeOf def Is Expr_LoopCountDownFixed Then
            'Temporary loop expression
            Dim tempExpr = DirectCast(def, Expr_LoopCountDownFixed)

            'Oringinal loop expression recreated
            Dim originalExp As New Expr_LoopFixed(tempExpr.NumberOfLoops)

            g_lstDefFlattened.Item(index) = originalExp
        End If

        If TypeOf def Is Expr_LoopCountDownRandom Then
            'Temporary loop expression
            Dim tempExpr = DirectCast(def, Expr_LoopCountDownRandom)

            'Oringinal loop expression recreated
            Dim originalExp As New Expr_LoopRandom(tempExpr.MinLoops, tempExpr.MaxLoops)

            g_lstDefFlattened.Item(index) = originalExp

        End If

    End Sub

    Private Function InternalGotoLine(ByVal index As Integer, ByVal resetLoops As Boolean) As Boolean
        Dim curState As String = ""
        Dim jumpSuccessful As Boolean = False

        For i = 0 To g_lstDefFlattened.Count - 1
            If resetLoops Then ResetLoopExpressionIfNeeded(i)

            Dim exp As DefinitionExpression = g_lstDefFlattened.Item(i)

            If TypeOf exp Is Expr_Label Then
                curState = DirectCast(exp, Expr_Label).LabelName
            End If

            If i = index Then
                g_iCurrentExprIndex = i
                g_sCurrentStateName = curState
                jumpSuccessful = True

                If Not resetLoops Then
                    Exit For
                End If

            End If
        Next
        Return jumpSuccessful
    End Function

    'Splits frame expression lines where multiple frames
    'are expressed in a single line into multiple single frame
    'expressions
    Private Function FlattenDefinition(ByVal definition As IList(Of DefinitionExpression)) As DefinitionExpression()
        Dim lstNewDef As New List(Of DefinitionExpression)

        For Each de In definition
            If TypeOf de Is Expr_Frames Then
                Dim fe As Expr_Frames = DirectCast(de, Expr_Frames)

                If fe.Frames.Length > 1 Then
                    For Each frame In fe.Frames
                        lstNewDef.Add(New Expr_Frames(fe.SpriteName, frame.ToString, fe.Delay, Nothing))
                    Next

                    'Put the function call on the last frame
                    If fe.ExecuteInternalFunction IsNot Nothing Then
                        DirectCast(lstNewDef.Last, Expr_Frames).ExecuteInternalFunction = fe.ExecuteInternalFunction
                    End If
                Else 'Expression is already a single frame expression


                    lstNewDef.Add(fe)
                End If
            Else
                lstNewDef.Add(de)
            End If

        Next

        Return lstNewDef.ToArray
    End Function

    Private Class TickTimer
        Public Sub New(ByVal handlerState As String, ByVal interval As Integer, ByVal fireOnce As Boolean)
            Me.FireOnce = fireOnce
            Me.CountDown = interval
            Me.Interval = interval
            Me.HandlerState = handlerState
        End Sub

        Public Property Interval As Integer

        'The state that it jumps to once
        'the timer has fired
        Public Property HandlerState As String
        Public Property FireOnce As Boolean
        Public Property CountDown As Integer
    End Class

    Private Class Expr_LoopCountDownBase
        Inherits DefinitionExpression

        Private _numLoops As Integer
        Public Sub New(ByVal numberOfLoops As Integer)
            Me.CurrentLoopCount = 0
            _numLoops = numberOfLoops
        End Sub

        Public ReadOnly Property NumberOfLoops As Integer
            Get
                Return _numLoops
            End Get
        End Property
        Public Property CurrentLoopCount As Integer
    End Class

    Private Class Expr_LoopCountDownRandom
        Inherits Expr_LoopCountDownBase

        Private _min As Integer
        Private _max As Integer

        Public Sub New(ByVal numLoops As Integer, ByVal minLoops As Integer, ByVal maxLoops As Integer)
            MyBase.New(numLoops)
            _max = maxLoops
            _min = minLoops
        End Sub

        Public ReadOnly Property MaxLoops As Integer
            Get
                Return _max
            End Get
        End Property

        Public ReadOnly Property MinLoops As Integer
            Get
                Return _min
            End Get
        End Property

    End Class

    Private Class Expr_LoopCountDownFixed
        Inherits Expr_LoopCountDownBase

        Public Sub New(ByVal numberOfLoops As Integer)
            MyBase.new(numberOfLoops)
        End Sub
    End Class

    Private Class ReturnPointer
        Public Sub New(ByVal state As String, ByVal index As Integer)
            Me.State = state
            Me.Index = index
        End Sub

        Public Property State As String

        'Index of the definition expression
        Public Property Index As Integer

    End Class

End Class