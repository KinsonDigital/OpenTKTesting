using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKTesting
{
    public class Game : GameWindow
    {
        private int _textureId;

        public Game(int width, int height) : base(width, height)
        {
            //Tell OpenGL that we want textures to be rendered when drawing polygons
            GL.Enable(EnableCap.Texture2D);

            //Next to lines below allow the blending for alpha channels of the image
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _textureId = TextureLoader.Load("gear.png");
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {

            //Clear the buffer bit of the color
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.CornflowerBlue);
            

            //Not aloud to change texture targets, (call below) between the Begin() and End() calls
            //This tells OpenGL that all of the GL primitive calls betw2een the Begin() and End() calls
            //will apply to this texture in the line below.
            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            GL.Begin(PrimitiveType.Quads);

            /*OpenGL Coordinate System
             0,0 = Center
             -1,-1 = Top Left Corner
             1,-1 = Top Right Corner
             1,1 = Bottom Right Corner
             -1, 1 = Bottom Left Corner

                pos x is to the right
                neg x is to the left
                pos y is up
                neg y is down
             */

            /*Color system
             Setting the GL.Color3() will set the color for all of the next consecutive calls
             to the GL.Vertex2() calls.  
             */
            GL.Color4(Color.FromArgb(255, 255, 0, 0));
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);

            GL.Color4(Color.FromArgb(255, 0, 0, 255));
            GL.TexCoord2(1, 0);
            GL.Vertex2(1, 0);

            GL.Color4(Color.FromArgb(255, 255, 125, 0));
            GL.TexCoord2(1, 1);
            GL.Vertex2(1, -1);

            GL.Color4(Color.FromArgb(255, 0, 255, 0));
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, -1);

            GL.End();

            SwapBuffers();

            base.OnRenderFrame(e);
        }
    }
}
