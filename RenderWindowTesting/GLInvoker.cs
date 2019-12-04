using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace RenderWindowTesting
{
    public class GLInvoker : IGLInvoker
    {
        public void UseShader(int shaderHandle) => GL.UseProgram(shaderHandle);


        public void DestroyShader(int program, int shaderHandle)
        {
            GL.DetachShader(program, shaderHandle);
            GL.DeleteShader(shaderHandle);
        }


        public int CreateShader(ShaderType shaderType, string shaderSrc)
        {
            var fragmentShader = GL.CreateShader(shaderType);
            GL.ShaderSource(fragmentShader, shaderSrc);
            CompileShader(fragmentShader);


            return fragmentShader;
        }


        public int CreateShaderProgram(int vertexShaderHandle, int fragmentShaderHandle)
        {
            var programHandle = GL.CreateProgram();

            //Attach both shaders...
            GL.AttachShader(programHandle, vertexShaderHandle);
            GL.AttachShader(programHandle, fragmentShaderHandle);

            //Link them together
            LinkProgram(programHandle);


            return programHandle;
        }


        public void SetVec4Uniform(int shaderProgramHandle, string uniformName, Vector4 vector)
        {
            //TODO: Make use of the cached uniform locations in the Shader class.
            var vec4Location = GL.GetUniformLocation(shaderProgramHandle, uniformName);

            if (vec4Location == -1)
                throw new Exception($"The uniform with the name '{uniformName}' does not exist.");

            GL.Uniform4(vec4Location, ref vector);
        }


        public void SetupVertexShaderAttribute(Shader shader, string attrName, int size, int offSet)
        {
            var vertexLocation = shader.GetAttribLocation(attrName);
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, size, VertexAttribPointerType.Float, false, 5 * sizeof(float), offSet);
        }


        public void SetMat4Uniform(int shaderProgramHandle, string uniformName, Matrix4 matrix)
        {
            var uniformTransformationLocation = GL.GetUniformLocation(shaderProgramHandle, uniformName);
            GL.UniformMatrix4(uniformTransformationLocation, true, ref matrix);
        }


        public void CompileShader(int shaderHandle)
        {
            // Try to compile the shader
            GL.CompileShader(shaderHandle);

            // Check for compilation errors
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out var code);

            if (code != (int)All.True)
            {
                var errorInfo = GL.GetShaderInfoLog(shaderHandle);
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                throw new Exception($"Error occurred while compiling Shader({shaderHandle})\n{errorInfo}");
            }
        }


        public void LinkProgram(int programHHandle)
        {
            // We link the program
            GL.LinkProgram(programHHandle);

            // Check for linking errors
            GL.GetProgram(programHHandle, GetProgramParameterName.LinkStatus, out var code);

            if (code != (int)All.True)
            {
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred whilst linking Program({programHHandle})");
            }
        }


        public void EnableAlpha()
        {
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }


        public void ClearColor(byte r, byte g, byte b, byte a)
        {
            var resultR = r.MapValue(0f, 255f, 0f, 1f);
            var resultG = g.MapValue(0f, 255f, 0f, 1f);
            var resultB = b.MapValue(0f, 255f, 0f, 1f);
            var resultA = a.MapValue(0f, 255f, 0f, 1f);

            GL.ClearColor(resultR, resultG, resultB, resultA);
        }
    }
}
