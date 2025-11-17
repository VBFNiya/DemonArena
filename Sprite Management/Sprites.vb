Public Class DemonSprites

    Private Shared _wraith As SpriteCollection
    Private Shared _spirit As SpriteCollection
    Private Shared _blueSpider As SpriteCollection
    Private Shared _yellowSpider As SpriteCollection
    Private Shared _cacolich As SpriteCollection
    Private Shared _blueAfrit As SpriteCollection
    Private Shared _purpleAfrit As SpriteCollection
    Private Shared _goldenAfrit As SpriteCollection
    Private Shared _ironLich As SpriteCollection
    Private Shared _fallen As SpriteCollection
    Private Shared _wizard As SpriteCollection
    Private Shared _darkCard As SpriteCollection
    Private Shared _lostSoul As SpriteCollection
    Private Shared _arachNorb As SpriteCollection
    Private Shared _disciple As SpriteCollection

    Public Shared ReadOnly Property Disciple As SpriteCollection
        Get
            If _disciple Is Nothing Then
                _disciple = SpriteLoader.LoadSprites(GetType(My.Resources.Disciple))
            End If

            Return _disciple
        End Get
    End Property



    Public Shared ReadOnly Property ArachNorb As SpriteCollection
        Get
            If _arachNorb Is Nothing Then
                _arachNorb = SpriteLoader.LoadSprites(GetType(My.Resources.ArachNorb))
            End If

            Return _arachNorb
        End Get
    End Property


    Public Shared ReadOnly Property YellowSpider As SpriteCollection
        Get
            If _yellowSpider Is Nothing Then
                _yellowSpider = SpriteLoader.LoadSprites(GetType(My.Resources.Spider2))
            End If

            Return _yellowSpider
        End Get
    End Property


    Public Shared ReadOnly Property LostSoul As SpriteCollection
        Get
            If _lostSoul Is Nothing Then
                _lostSoul = SpriteLoader.LoadSprites(GetType(My.Resources.LostSoul))
            End If

            Return _lostSoul
        End Get
    End Property


    Public Shared ReadOnly Property DarkCardinal As SpriteCollection
        Get
            If _darkCard Is Nothing Then
                _darkCard = SpriteLoader.LoadSprites(GetType(My.Resources.DarkCardinal))
            End If

            Return _darkCard
        End Get
    End Property

    Public Shared ReadOnly Property Wizard As SpriteCollection
        Get
            If _wizard Is Nothing Then
                _wizard = SpriteLoader.LoadSprites(GetType(My.Resources.Wizard))
            End If

            Return _wizard
        End Get
    End Property

    Public Shared ReadOnly Property Fallen As SpriteCollection
        Get
            If _fallen Is Nothing Then
                _fallen = SpriteLoader.LoadSprites(GetType(My.Resources.Fallen))
            End If

            Return _fallen
        End Get
    End Property

    Public Shared ReadOnly Property IronLich As SpriteCollection
        Get
            If _ironLich Is Nothing Then
                _ironLich = SpriteLoader.LoadSprites(GetType(My.Resources.Ironlich))
            End If

            Return _ironLich
        End Get
    End Property

    Public Shared ReadOnly Property GoldAfrit As SpriteCollection
        Get
            If _goldenAfrit Is Nothing Then
                _goldenAfrit = SpriteLoader.LoadSprites(GetType(My.Resources.Afrit3))
            End If

            Return _goldenAfrit
        End Get
    End Property

    Public Shared ReadOnly Property PurpleAfrit As SpriteCollection
        Get
            If _purpleAfrit Is Nothing Then
                _purpleAfrit = SpriteLoader.LoadSprites(GetType(My.Resources.Afrit2))
            End If

            Return _purpleAfrit
        End Get
    End Property

    Public Shared ReadOnly Property BlueAfrit As SpriteCollection
        Get
            If _blueAfrit Is Nothing Then
                _blueAfrit = SpriteLoader.LoadSprites(GetType(My.Resources.Afrit))
            End If

            Return _blueAfrit
        End Get
    End Property

    Public Shared ReadOnly Property Cacolich As SpriteCollection
        Get

            If _cacolich Is Nothing Then
                _cacolich = SpriteLoader.LoadSprites(GetType(My.Resources.Cacolich))
            End If

            Return _cacolich
        End Get
    End Property


    Public Shared ReadOnly Property Wraith As SpriteCollection
        Get

            If _wraith Is Nothing Then
                _wraith = SpriteLoader.LoadSprites(GetType(My.Resources.Wraith))
            End If

            Return _wraith
        End Get
    End Property

    Public Shared ReadOnly Property Spirit As SpriteCollection
        Get

            If _spirit Is Nothing Then
                _spirit = SpriteLoader.LoadSprites(GetType(My.Resources.Spirit))
            End If

            Return _spirit
        End Get
    End Property

    Public Shared ReadOnly Property BlueSpider As SpriteCollection
        Get

            If _Bluespider Is Nothing Then
                _Bluespider = SpriteLoader.LoadSprites(GetType(My.Resources.Spider))
            End If

            Return _Bluespider
        End Get
    End Property


End Class

Public Class ProjectileSprites

    Private Shared _wraithProj As SpriteCollection
    Private Shared _spiritProj As SpriteCollection
    Private Shared _spiderProjWeak As SpriteCollection
    Private Shared _spiderProjStrong As SpriteCollection
    Private Shared _cacolichProj As SpriteCollection
    Private Shared _cometProj As SpriteCollection
    Private Shared _afritProjWeak As SpriteCollection
    Private Shared _ironLichProj As SpriteCollection
    Private Shared _fallenProj As SpriteCollection
    Private Shared _wizProj As SpriteCollection
    Private Shared _rocket As SpriteCollection
    Private Shared _greenPlasmaBall As SpriteCollection
    Private Shared _arachNorbProj As SpriteCollection
    Private Shared _discipleProj1 As SpriteCollection

    Public Shared ReadOnly Property DiscipleProjectile1 As SpriteCollection
        Get
            If _discipleProj1 Is Nothing Then
                _discipleProj1 = SpriteLoader.LoadSprites(GetType(My.Resources.DiscipleProjectile1))
            End If

            Return _discipleProj1
        End Get
    End Property


    Public Shared ReadOnly Property ArachNorbProjectile As SpriteCollection
        Get
            If _arachNorbProj Is Nothing Then
                _arachNorbProj = SpriteLoader.LoadSprites(GetType(My.Resources.ArachNorbProjectile))
            End If

            Return _arachNorbProj
        End Get
    End Property

    Public Shared ReadOnly Property GreenPlasmaBall As SpriteCollection
        Get
            If _greenPlasmaBall Is Nothing Then
                _greenPlasmaBall = SpriteLoader.LoadSprites(GetType(My.Resources.PlasmaBall))
            End If

            Return _greenPlasmaBall
        End Get
    End Property


    Public Shared ReadOnly Property Rocket As SpriteCollection
        Get
            If _rocket Is Nothing Then
                _rocket = SpriteLoader.LoadSprites(GetType(My.Resources.Rocket))
            End If

            Return _rocket
        End Get
    End Property


    Public Shared ReadOnly Property WizardProjectile As SpriteCollection
        Get
            If _wizProj Is Nothing Then
                _wizProj = SpriteLoader.LoadSprites(GetType(My.Resources.WizardProjectile))
            End If

            Return _wizProj
        End Get
    End Property


    Public Shared ReadOnly Property FallenProjectile As SpriteCollection
        Get
            If _fallenProj Is Nothing Then
                _fallenProj = SpriteLoader.LoadSprites(GetType(My.Resources.FallenProjectile), Color.White)
            End If

            Return _fallenProj
        End Get
    End Property


    Public Shared ReadOnly Property IronLichProjectile As SpriteCollection
        Get
            If _ironLichProj Is Nothing Then
                _ironLichProj = SpriteLoader.LoadSprites(GetType(My.Resources.IronLichProjectile))
            End If

            Return _ironLichProj
        End Get
    End Property


    Public Shared ReadOnly Property AfritProjectileWeak As SpriteCollection
        Get
            If _afritProjWeak Is Nothing Then
                _afritProjWeak = SpriteLoader.LoadSprites(GetType(My.Resources.AfritWeakProjectile))
            End If

            Return _afritProjWeak
        End Get
    End Property


    Public Shared ReadOnly Property CometProjectile As SpriteCollection
        Get
            If _cometProj Is Nothing Then
                _cometProj = SpriteLoader.LoadSprites(GetType(My.Resources.CometProjectile), Color.White)
            End If

            Return _cometProj
        End Get
    End Property

    Public Shared ReadOnly Property CacolichProjectile As SpriteCollection
        Get
            If _cacolichProj Is Nothing Then
                _cacolichProj = SpriteLoader.LoadSprites(GetType(My.Resources.CacolichProjectile), Color.White)
            End If

            Return _cacolichProj
        End Get
    End Property

    Public Shared ReadOnly Property SpiderProjectileStrong As SpriteCollection
        Get
            If _spiderProjStrong Is Nothing Then
                _spiderProjStrong = SpriteLoader.LoadSprites(GetType(My.Resources.SpiderProjectileStrong))
            End If

            Return _spiderProjStrong
        End Get
    End Property


    Public Shared ReadOnly Property SpiderProjectileWeak As SpriteCollection
        Get
            If _spiderProjWeak Is Nothing Then
                _spiderProjWeak = SpriteLoader.LoadSprites(GetType(My.Resources.SpiderProjectileWeak))
            End If

            Return _spiderProjWeak
        End Get
    End Property


    Public Shared ReadOnly Property SpiritProjectile As SpriteCollection
        Get
            If _spiritProj Is Nothing Then
                _spiritProj = SpriteLoader.LoadSprites(GetType(My.Resources.SpiritProjectile))
            End If

            Return _spiritProj
        End Get
    End Property

    Public Shared ReadOnly Property WraithProjectile As SpriteCollection
        Get

            If _wraithProj Is Nothing Then
                _wraithProj = SpriteLoader.LoadSprites(GetType(My.Resources.WraithProjectile))
            End If

            Return _wraithProj
        End Get
    End Property
End Class

Public Class AnomolySprites
    Private Shared _teleFog As SpriteCollection
    Private Shared _blueExplosion As SpriteCollection

    Public Shared ReadOnly Property BlueExplosion As SpriteCollection
        Get
            If _blueExplosion Is Nothing Then
                _blueExplosion = SpriteLoader.LoadSprites(GetType(My.Resources.Explosion))
            End If

            Return _blueExplosion
        End Get
    End Property

    Public Shared ReadOnly Property TeleportFog As SpriteCollection
        Get
            If _teleFog Is Nothing Then
                _teleFog = SpriteLoader.LoadSprites(GetType(My.Resources.TeleportFog), Color.Cyan)
            End If

            Return _teleFog
        End Get
    End Property

End Class

Public Class Glows

    'We don't want to have to recreate specific
    'gradients every time. Animations can take
    'a slight performance hit if we have to create
    'gradients too many time so we use a simple look
    'up to get gradients that were already created
    Private Shared _cache As New List(Of RadGrad)

    Public Shared Function GetRadialGradient(ByVal color As Color) As Bitmap
        Return GetRadialGradient(color, New Size(50, 50))
    End Function

    Public Shared Function GetRadialGradient(ByVal color As Color, ByVal size As Size) As Bitmap
        'Check the cache first
        Dim g As RadGrad = _cache.FirstOrDefault(Function(g1) g1.Color = color AndAlso g1.Size = size)

        If g Is Nothing Then
            'It wasn't in the cache so we create it
            Dim b As Bitmap = DrawingHelpers.CreateRadialGradient(color, size)

            'Create an object to cache so we can
            'recall the gradient later
            g = New RadGrad With {.Bitmap = b, .Color = color, .Size = size}

            'Cache it
            _cache.Add(g)
        End If

        Return g.Bitmap
    End Function


    Private Class RadGrad
        Public Property Color As Color
        Public Property Size As Size
        Public Property Bitmap As Bitmap
    End Class

End Class

'Public Class Glows

'    Shared Sub New()
'        Red = My.Resources.Glows.LM_Red
'        Red.MakeTransparent(Color.Black)

'        Blue = My.Resources.Glows.LM_Blue
'        Blue.MakeTransparent(Color.Black)

'        Green = My.Resources.Glows.LM_Green
'        Green.MakeTransparent(Color.Black)
'    End Sub

'    Public Shared Red As Bitmap
'    Public Shared Blue As Bitmap
'    Public Shared Green As Bitmap


'End Class

