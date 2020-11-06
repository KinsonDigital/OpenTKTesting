using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKTesting
{
    // A simple class meant to help create shaders.
    public class Shader : IDisposable
    {
        public readonly int ProgramHandle;

        private static readonly Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();


        #region Constructors
        // This is how you create a simple shader.
        // Shaders are written in GLSL, which is a language very similar to C in its semantics.
        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        // A commented example of GLSL can be found in shader.vert
        public Shader(string vertPath, string fragPath)
        {
            // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
            // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
            //   The vertex shader won't be too important here, but they'll be more important later.
            // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
            //   The fragment shader is what we'll be using the most here.

            // Load vertex shader and compile
            // LoadSource is a simple function that just loads all text from the file whose path is given.
            var shaderSource = LoadSource(vertPath);
            VertexShaderHandle = GLExt.CreateShader(ShaderType.VertexShader, shaderSource);

            // We do the same for the fragment shader
            shaderSource = LoadSource(fragPath);
            FragmentShaderHandle = GLExt.CreateShader(ShaderType.FragmentShader, shaderSource);

            // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
            // To do this, create a program...
            ProgramHandle = GLExt.CreateShaderProgram(VertexShaderHandle, FragmentShaderHandle);

            // When the shader program is linked, it no longer needs the individual shaders attacked to it; the compiled code is copied into the shader program.
            // Detach them, and then delete them.
            GLExt.DestroyShader(ProgramHandle, VertexShaderHandle);
            GLExt.DestroyShader(ProgramHandle, FragmentShaderHandle);

            // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(ProgramHandle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(ProgramHandle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(ProgramHandle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }
        }
        #endregion


        #region Props
        public int VertexShaderHandle { get; private set; }

        public int FragmentShaderHandle { get; private set; }
        #endregion


        // A wrapper function that enables the shader program.
        public void Use()
        {
            GLExt.UseShader(ProgramHandle);
        }


        // The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
        // you can omit the layout(location=X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(ProgramHandle, attribName);
        }


        // Just loads the entire file into a string.
        private static string LoadSource(string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        // Uniform setters
        // Uniforms are variables that can be set by user code, instead of reading them from the VBO.
        // You use VBOs for vertex-related data, and uniforms for almost everything else.

        // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
        //     1. Bind the program you want to set the uniform on
        //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
        //     3. Use the appropriate GL.Uniform* function to set the uniform.

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            GL.UseProgram(ProgramHandle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(ProgramHandle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(ProgramHandle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(ProgramHandle);
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void Dispose()
        {
            GL.DeleteProgram(ProgramHandle);
        }
    }
}
