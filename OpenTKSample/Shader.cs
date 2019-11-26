using System;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace OpenTKSample
{
    public class Shader : IDisposable
    {
        private bool _disposedValue;

        public int Program { get; }

        public Shader(string vertexPath, string fragmentPath)
        {
            var basePath = @"Shaders\";
            var fullVertexPath = $@"{basePath}\{vertexPath}";
            var fullFragPath = $@"{basePath}\{fragmentPath}";

            var vertexShaderSrc = string.Empty;//Vertex shader source code
            var fragShaderSrc = string.Empty;//Fragment shader source code

            using (var reader = new StreamReader(fullVertexPath, Encoding.UTF8))
            {
                vertexShaderSrc = reader.ReadToEnd();
            }


            using (var reader = new StreamReader(fullFragPath, Encoding.UTF8))
            {
                fragShaderSrc = reader.ReadToEnd();
            }

            //Create the vertex shader object
            var vertexShaderObj = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderObj, vertexShaderSrc);//Bind the vertex shader source code to the vertex shader object

            //Create the fragment shader object
            var fragShaderObj = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShaderObj, fragShaderSrc);//Bind the fragment shader source code to the fragment shader object

            //Compile the vertex shader
            GL.CompileShader(vertexShaderObj);

            //Check if there was an error compiling the vertex shader source code
            var vertInfoLog = GL.GetShaderInfoLog(vertexShaderObj);
            if (vertInfoLog != string.Empty)
                throw new Exception($"Vertex Shader Compile Error: {vertInfoLog}");

            //Compile the frag shader
            GL.CompileShader(fragShaderObj);

            //Check if there was an error compiling the frag shader source code
            var fragInfoLog = GL.GetShaderInfoLog(fragShaderObj);
            if (fragInfoLog != string.Empty)
                throw new Exception($"Fragment Shader Compile Error: {fragInfoLog}");

            //Create a shader program handle that represents both the vertex and fragment shaders as a single shader result
            Program = GL.CreateProgram();

            GL.AttachShader(Program, vertexShaderObj);
            GL.AttachShader(Program, fragShaderObj);

            //Link both the vertex and fragment shaders into a single shader program that can run on the GPU
            GL.LinkProgram(Program);

            //Cleanup the shader data.  It has been copied as a program to the GPU and is not needed anymore
            GL.DetachShader(Program, vertexShaderObj);
            GL.DetachShader(Program, fragShaderObj);
            GL.DeleteShader(vertexShaderObj);
            GL.DeleteShader(fragShaderObj);
        }


        ~Shader() => GL.DeleteProgram(Program);


        /// <summary>
        /// Invoking this will tell the GPU to use the created shader
        /// </summary>
        public void Use() => GL.UseProgram(Program);


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                GL.DeleteProgram(Program);
                _disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
