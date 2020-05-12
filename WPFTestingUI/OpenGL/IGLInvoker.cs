using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WPFTestingUI.OpenGL
{
    public interface IGLInvoker
    {
        void UseShader(int shaderHandle);


        void DestroyShader(int program, int shaderHandle);


        int CreateShader(ShaderType shaderType, string shaderSrc);


        int CreateShaderProgram(int vertexShaderHandle, int fragmentShaderHandle);


        void SetVec4Uniform(int shaderProgramHandle, string uniformName, Vector4 vector);


        void SetupVertexShaderAttribute(Shader shader, string attrName, int size, int offSet);


        void SetMat4Uniform(int shaderProgramHandle, string uniformName, Matrix4 matrix);


        void CompileShader(int shaderHandle);


        void LinkProgram(int programHHandle);


        void EnableAlpha();


        void ClearColor(byte r, byte g, byte b, byte a);
    }
}
