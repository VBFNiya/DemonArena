Imports System.Drawing.Imaging
Imports Microsoft.Xna.Framework.Graphics
Imports System.ComponentModel

Public Class Arena
    Inherits Control
    Implements IArena

    Public Event EntityListChanged As EventHandler(Of EntityListChangedEventArgs) Implements IArena.EntityListChanged
    Protected Sub OnEntityListChanged(ByVal e As EntityListChangedEventArgs)
        RaiseEvent EntityListChanged(Me, e)
    End Sub

    Private g_dictCachedGlows As New Dictionary(Of String, CachedGlowImage)
    Private g_dictTextures As New Dictionary(Of String, Texture2D)

    Private g_lstEntities As New List(Of IArenaEntity)

    'This is a read-only collection meant to be exposed
    'via a public propery
    Private g_colROEntities As EntityCollection
    Private g_objRnd As New Random

    Private g_bmpBackgroundOriginal As Bitmap

    Private g_xnaGraphicsDevice As GraphicsDevice
    Private g_xnaSpriteBatch As SpriteBatch

    Private g_bDevicePrepared As Boolean = False

    Private g_bmpBackgroundResized As Bitmap
    Private g_hBitmapBKGrnd As ManagedGDI.ManagedhBitmap
    Private g_texBkGrnd As Texture2D

    Private g_dblScaleFactor As Double
    Private g_bSizeSet As Boolean
    Private g_szArenaSize As Size

    Private g_swFPS As New Stopwatch
    Private g_iFrameCount As Integer
    Private g_iFPS As Integer

    Public Sub New()
        PrepareDevice()

        Me.DoubleBuffered = True

        Me.GlowEffectsOn = True

        Me.UseXNA = False
    End Sub

    Public ReadOnly Property CurrentFrameRate As Integer
        Get
            Return g_iFPS
        End Get
    End Property


    Public Sub AddEntity(ByVal entity As IArenaEntity) Implements IArena.AddEntity
        entity.Arena = Me

        'We want projectiles to be drawn first
        If TypeOf entity Is Projectile Then
            g_lstEntities.Add(entity)
        Else
            g_lstEntities.Add(entity)

            If TypeOf entity Is IDemon Then
                Dim fog As New TeleportFog

                fog.Arena = Me
                fog.SetPosition(entity.Position)

                g_lstEntities.Add(fog)
            End If
        End If

        If g_colROEntities Is Nothing Then
            g_colROEntities = New EntityCollection(g_lstEntities)
        End If

        OnEntityListChanged(New EntityListChangedEventArgs(entity, EntityListChangeType.EntityAdded))

    End Sub

    Public Property GlowEffectsOn As Boolean

    Public Property UseXNA As Boolean

    Public Property ArenaBackground As System.Drawing.Bitmap Implements IArena.ArenaBackground
        Get
            Return g_bmpBackgroundOriginal
        End Get
        Set(ByVal value As System.Drawing.Bitmap)
            g_bmpBackgroundOriginal = value

            If g_texBkGrnd IsNot Nothing Then
                g_texBkGrnd.Dispose()
                g_texBkGrnd = Nothing
            End If

            If g_hBitmapBKGrnd IsNot Nothing Then
                g_hBitmapBKGrnd.Dispose()
                g_hBitmapBKGrnd = Nothing

            End If

            If value Is Nothing Then
                g_bmpBackgroundResized = Nothing
            Else
                g_bmpBackgroundResized = New Bitmap(value, Me.Size)
                g_hBitmapBKGrnd = New ManagedGDI.ManagedhBitmap(g_bmpBackgroundResized)

                PrepareBackgroundXNA(g_bmpBackgroundResized)
            End If

        End Set
    End Property

    Private Sub PrepareBackgroundXNA(ByVal bkImg As Bitmap)
        If g_texBkGrnd IsNot Nothing Then g_texBkGrnd.Dispose()
        g_texBkGrnd = DrawingHelpers.BitmapToTexture2D(g_xnaGraphicsDevice, bkImg)
    End Sub

    Public Property ArenaSize As System.Drawing.Size Implements IArena.ArenaSize
        Get
            If g_szArenaSize.IsEmpty Then
                g_szArenaSize = Me.Size
            End If

            Return g_szArenaSize
        End Get
        Set(ByVal value As System.Drawing.Size)
            g_szArenaSize = value
        End Set
    End Property

    Public Sub CallTick() Implements IArena.CallTick
        If Not g_swFPS.IsRunning Then
            g_swFPS.Start()
        End If

        Dim counter As Integer = 0

        Do Until counter >= g_lstEntities.Count

            Dim e As IArenaEntity = g_lstEntities.Item(counter)
            e.Tick()

            counter += 1
        Loop

        If Not Me.UseXNA Then
            Me.Refresh()
        Else
            Me.RefreshXNA()
        End If

        g_iFrameCount += 1

        If g_swFPS.ElapsedMilliseconds >= 1000 Then
            Debug.WriteLine("FPS:" + g_iFrameCount.ToString)
            g_iFPS = g_iFrameCount
            g_iFrameCount = 0
            g_swFPS.Reset()
        End If

    End Sub

    Public ReadOnly Property Entities As EntityCollection Implements IArena.Entities
        Get
            If g_colROEntities Is Nothing Then
                g_colROEntities = New EntityCollection(g_lstEntities)
            End If

            Return g_colROEntities
        End Get
    End Property

    Public Sub RemoveEntity(ByVal entity As IArenaEntity) Implements IArena.RemoveEntity
        g_lstEntities.Remove(entity)

        entity.Arena = Nothing

        OnEntityListChanged(New EntityListChangedEventArgs(entity, EntityListChangeType.EntityRemoved))
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        If Not Me.UseXNA Then

            Dim arenaScale As ArenaScale = GetArenaScaleFactor()

            If g_bmpBackgroundResized IsNot Nothing Then
                GDIPlusDrawImage(e.Graphics, g_bmpBackgroundResized, New RectangleF(0, 0, Me.Width, Me.Height), 1)
            End If

            For Each entity As IArenaEntity In g_lstEntities.Where(Function(ee) ee.RenderInfo IsNot Nothing)

                If TypeOf entity Is IGlowingEntity AndAlso Me.GlowEffectsOn Then
                    'For use in lambda
                    Dim entity1 As IArenaEntity = entity

                    Dim glEnt As IGlowingEntity = DirectCast(entity, IGlowingEntity)
                    Dim key = CreateCacheKey(entity1.RenderInfo.Sprite.SpriteName, entity1.Bounds.Size.ToSize)

                    Dim cg As CachedGlowImage = Nothing

                    g_dictCachedGlows.TryGetValue(key, cg)

                    If cg Is Nothing Then

                        'Create a cached glow
                        cg = CreateCachedGlowObject(entity.RenderInfo.Sprite, entity.Bounds, glEnt.GlowColor, 10)

                        'Cache it
                        g_dictCachedGlows.Add(cg.Key, cg)
                    End If

                    'The bounds for rendering our blended image
                    Dim renderBounds As RectangleF = New RectangleF(0, 0, cg.Image.Width, cg.Image.Height)

                    'The blended image would be larger than the orginal
                    'image so we need to center it over the bounds of
                    'the original so it would render in the correct
                    'position
                    renderBounds.CenterRectInRect(entity.Bounds)

                    renderBounds = ScaleRect(renderBounds, arenaScale.WidthScale, arenaScale.HeightScale)

                    GDIPlusDrawImage(e.Graphics, cg.Image, renderBounds, 1)
                Else
                    With entity
                        Dim b As RectangleF = ScaleRect(.Bounds, arenaScale.WidthScale, arenaScale.HeightScale)

                        GDIPlusDrawImage(e.Graphics, .RenderInfo.Sprite, b, 1)
                    End With
                End If
            Next

        End If
        ControlPaint.DrawBorder(e.Graphics, Me.ClientRectangle, Color.Black, ButtonBorderStyle.Dotted)
    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        ResizeXNA()

        If Me.ArenaBackground IsNot Nothing Then
            If Me.Size <> New Size(0, 0) Then
                g_bmpBackgroundResized = New Bitmap(Me.ArenaBackground, Me.Size)

                If g_hBitmapBKGrnd IsNot Nothing Then
                    g_hBitmapBKGrnd.Dispose()
                    g_hBitmapBKGrnd = New ManagedGDI.ManagedhBitmap(g_bmpBackgroundResized)
                End If

                If g_texBkGrnd IsNot Nothing Then
                    g_texBkGrnd.Dispose()
                    g_texBkGrnd = DrawingHelpers.BitmapToTexture2D(g_xnaGraphicsDevice, g_bmpBackgroundResized)
                End If

            End If
        End If

        MyBase.OnResize(e)
    End Sub

    Private Sub ResizeXNA(ByVal size As Size)

        Dim pp As New PresentationParameters
        pp.DeviceWindowHandle = Me.Handle
        pp.IsFullScreen = False

        pp.BackBufferHeight = size.Height
        pp.BackBufferWidth = size.Width

        If pp.BackBufferWidth <= 0 Then pp.BackBufferWidth = 100
        If pp.BackBufferHeight <= 0 Then pp.BackBufferHeight = 100

        g_xnaGraphicsDevice.Reset(pp)

    End Sub

    Private Sub ResizeXNA()
        ResizeXNA(New Size(Me.ClientRectangle.Width, Me.ClientRectangle.Height))
    End Sub


    Private Sub RefreshXNA()

        'Scale ratios according to the ratio of the
        'control size to the arena size
        Dim arenaScaling = GetArenaScaleFactor()

        g_xnaSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied)

        XNADrawImage(g_xnaSpriteBatch, g_texBkGrnd, Me.ClientRectangle, 1)

        For Each entity In g_lstEntities
            If entity.RenderInfo IsNot Nothing Then
                Dim tex As Texture2D = entity.RenderInfo.Sprite.XNAImage

                If tex Is Nothing Then
                    tex = DrawingHelpers.BitmapToTexture2D(g_xnaGraphicsDevice, entity.RenderInfo.Sprite.Image)
                    entity.RenderInfo.Sprite.SetXNAImage(tex)
                End If

                'If Not g_dictTextures.TryGetValue(entity.RenderInfo.Sprite.SpriteName, tex) Then
                '    tex = DrawingHelpers.BitmapToTexture2D(g_xnaGraphicsDevice, entity.RenderInfo.Sprite.Image)

                '    g_dictTextures.Add(entity.RenderInfo.Sprite.SpriteName, tex)
                'End If

                Dim bounds As RectangleF = ScaleRect(entity.Bounds, arenaScaling.WidthScale, arenaScaling.HeightScale)

                XNADrawImage(g_xnaSpriteBatch, tex, bounds, entity.RenderInfo.Alpha)

                If Me.GlowEffectsOn AndAlso TypeOf entity Is IGlowingEntity Then
                    Dim glEnt As IGlowingEntity = DirectCast(entity, IGlowingEntity)

                    Dim glowTexture As Texture2D = entity.RenderInfo.Sprite.XNAGlowImage
                    'Dim key As String = CreateNameSizeKey(glEnt.GlowImageBounds.Size, glEnt.GlowColor.ToString)

                    If glowTexture Is Nothing Then
                        Dim bmp = DrawingHelpers.CreateRadialGradient(glEnt.GlowColor, glEnt.GlowImageBounds.Size.ToSize)

                        glowTexture = DrawingHelpers.BitmapToTexture2D(g_xnaGraphicsDevice, bmp)
                        entity.RenderInfo.Sprite.SetXNAGlowImg(glowTexture)
                    End If

                    'If Not g_dictTextures.TryGetValue(key, glowTexture) Then
                    '    Dim bmp = DrawingHelpers.CreateRadialGradient(glEnt.GlowColor, glEnt.GlowImageBounds.Size.ToSize)

                    '    glowTexture = DrawingHelpers.BitmapToTexture2D(g_xnaGraphicsDevice, bmp)

                    '    g_dictTextures.Add(key, glowTexture)
                    'End If

                    bounds = ScaleRect(glEnt.GlowImageBounds, arenaScaling.WidthScale, arenaScaling.HeightScale)

                    XNADrawImage(g_xnaSpriteBatch, glowTexture, bounds, 0.7)
                End If
            End If
        Next

        g_xnaSpriteBatch.End()


        Try
            g_xnaGraphicsDevice.Present()

            'Device can be lost if the graphics driver crashes or 
            'the resolution on the Windows Desktop is changed
        Catch ex As DeviceLostException
            PrepareDevice()

            'Dispose of all Texture2D objects created with the
            'last device
            Sprite.DisposeXNAOnAllSprites()

            'Dispose of the Texture2D background created with the last device
            PrepareBackgroundXNA(g_bmpBackgroundResized)

            ResizeXNA()
        End Try



    End Sub

    Private Function CreateNameSizeKey(ByVal size As SizeF, ByVal name As String) As String
        Return name + "_" + size.Width.ToString + "_" + size.Height.ToString
    End Function

    'Additional credit: dday9 @ VBForums.com
    Private Sub PrepareDevice()
        If g_xnaGraphicsDevice IsNot Nothing Then
            g_dictTextures.Clear()
            g_xnaGraphicsDevice.Dispose()
        End If

        If g_xnaSpriteBatch IsNot Nothing Then
            g_xnaSpriteBatch.Dispose()
        End If

        Dim pparam As New PresentationParameters
        pparam.DeviceWindowHandle = Me.Handle
        pparam.IsFullScreen = False

        pparam.BackBufferWidth = 100
        pparam.BackBufferHeight = 100


        Try
            'Try the HiDef profile first
            g_xnaGraphicsDevice = New GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, pparam)
        Catch ex As Exception
            'If that didn't work try the Reach profile which is lower spec
            g_xnaGraphicsDevice = New GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, pparam)
        End Try

        g_xnaSpriteBatch = New SpriteBatch(g_xnaGraphicsDevice)

        g_bDevicePrepared = True
    End Sub

    Private Sub GDIDrawImage(ByVal g As Graphics, ByVal image As Bitmap, ByVal imageRect As RectangleF, ByVal alpha As Single)
        ManagedGDI.AlphaBlend(image, g, imageRect.ToRectangle, alpha * 255)
    End Sub

    Private Sub GDIDrawImage(ByVal Dc As ManagedGDI.ManagedDc, ByVal image As ManagedGDI.ManagedhBitmap, ByVal imageRect As RectangleF, ByVal alpha As Single)
        ManagedGDI.AlphaBlend(image, Dc, imageRect.ToRectangle, alpha * 255)
    End Sub

    Private Sub GDIDrawImage(ByVal g As Graphics, ByVal image As ManagedGDI.ManagedhBitmap, ByVal imageRect As RectangleF, ByVal alpha As Single)
        ManagedGDI.AlphaBlend(image, g, imageRect.ToRectangle, alpha * 255)
    End Sub

    Private Sub XNADrawImage(ByVal sb As SpriteBatch, ByVal image As Texture2D, ByRef imageRect As RectangleF, ByVal alpha As Single)
        Dim r As New Microsoft.Xna.Framework.Rectangle(imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height)

        Dim c As New Microsoft.Xna.Framework.Color(255, 255, 255)
        c.A = alpha * 255

        sb.Draw(image, r, c)
    End Sub

    Private Sub GDIPlusDrawImage(ByVal g As Graphics, ByVal image As Bitmap, ByVal imageRect As RectangleF, ByVal alpha As Single)

        Dim attr As New ImageAttributes

        If alpha = 1 Then
            g.DrawImage(image, imageRect)
        Else
            attr = New ImageAttributes
            attr.SetColorMatrix(GetColorMatrixForAlpha(alpha))

            g.DrawImage(image, imageRect.ToRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attr)
        End If

    End Sub

    'Creates the matrix to set the alpha
    'value of the image that's fading in
    Private Function GetColorMatrixForAlpha(ByVal alpha As Single) As ColorMatrix

        Dim matrixItems As Single()() = { _
           New Single() {1, 0, 0, 0, 0}, _
           New Single() {0, 1, 0, 0, 0}, _
           New Single() {0, 0, 1, 0, 0}, _
           New Single() {0, 0, 0, alpha, 0}, _
           New Single() {0, 0, 0, 0, 1}}

        Return New ColorMatrix(matrixItems)
    End Function

    Private Function CreateCacheKey(ByVal spriteName As String, ByVal size As Size) As String
        Return spriteName + "_" + size.Width.ToString + "_" + size.Height.ToString
    End Function

    Private Function CreateCachedGlowObject(ByVal sprite As Sprite, ByVal entityRenderBounds As RectangleF, ByVal glowColor As Color, ByVal glowSize As Integer) As CachedGlowImage

        'Resize the sprite image to the size its
        'actually rendering as
        Dim image As Bitmap = New Bitmap(sprite, entityRenderBounds.Size.ToSize)

        'Create the canvas upon which we will paint our blended image
        Dim bmp As New Bitmap(image.Width + glowSize, image.Height + glowSize)

        Using g As Graphics = Graphics.FromImage(bmp)
            Dim imageRectF As New RectangleF(0, 0, image.Width, image.Height)

            'The canvas is larger than the image itself to leave
            'room for a glow around the image itself. We center
            'the image in the canvas
            imageRectF.CenterRectInRect(New RectangleF(0, 0, bmp.Width, bmp.Height))

            'Draw the image
            g.DrawImage(image, imageRectF)

            Dim attr As New ImageAttributes
            Dim glowBmp As Bitmap = Glows.GetRadialGradient(glowColor, bmp.Size)

            attr.SetColorMatrix(GetColorMatrixForAlpha(0.7))

            'Blend the glow
            g.DrawImage(glowBmp, New Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, glowBmp.Width, glowBmp.Height, GraphicsUnit.Pixel, attr)
        End Using

        Return New CachedGlowImage(CreateCacheKey(sprite.SpriteName, entityRenderBounds.Size.ToSize), bmp, glowSize)
    End Function

    Private Function ScaleRect(ByVal rect As RectangleF, ByVal scaleX As Double, ByVal scaleY As Double) As RectangleF

        rect.X *= scaleX
        rect.Y *= scaleY
        rect.Width *= scaleX
        rect.Height *= scaleY

        Return rect
    End Function

    Private Function GetArenaScaleFactor() As ArenaScale

        Dim widthScale As Double = Me.ClientRectangle.Width / Me.ArenaSize.Width
        Dim heightScale As Double = Me.ClientRectangle.Height / Me.ArenaSize.Height

        Return New ArenaScale(widthScale, heightScale)
    End Function

    Private Structure ArenaScale
        Public Sub New(ByVal ws As Double, ByVal hs As Double)
            Me.WidthScale = ws
            Me.HeightScale = hs
        End Sub

        Public Property WidthScale As Double
        Public Property HeightScale As Double

    End Structure



End Class

Public Class CachedGlowImage

    Private _GDIImage As ManagedGDI.ManagedhBitmap

    Public Sub New(ByVal key As String, ByVal image As Bitmap, ByVal glowSize As Integer)
        Me.Key = key
        Me.Image = image
        Me.GlowSize = glowSize

        If Me.Image IsNot Nothing Then
            _GDIImage = New ManagedGDI.ManagedhBitmap(Me.Image)
        End If
    End Sub

    Public Property Key As String
    Public Property Image As Bitmap
    Public ReadOnly Property GDIImage As ManagedGDI.ManagedhBitmap
        Get
            Return _GDIImage
        End Get
    End Property
    Public Property GlowSize As Integer

End Class

Public Class CollisionGrid

    Public Sub New(ByVal arenaRect As Rectangle, ByVal xGridCount As Integer, ByVal yGridCount As Integer)

    End Sub

    Public Sub AddEntity(ByVal e As IArenaEntity)

    End Sub

End Class