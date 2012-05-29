using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using libASEsharp.Lexer;

namespace libASEsharp
{
    public class Parser
    {
        private Scanner scanner;

        private bool readInt(out int ret)
        {
            if (scanner.yylex() != (int)TokenType.Integer)
            {
                Console.Error.WriteLine("Expected an integer");
                ret = 0;
                return false;
            }
            ret = int.Parse(scanner.yytext);
            return true;
        }

        private bool readBool(out bool ret)
        {
            int i;
            ret = false;
            if (!readInt(out i)) return false;
            if (i == 1) ret = true;
            return true;
        }

        private bool readFloat(out float ret)
        {
            if (scanner.yylex() != (int)TokenType.Float)
            {
                Console.Error.WriteLine("Expected a float");
                ret = 0;
                return false;
            }
            ret = float.Parse(scanner.yytext);
            return true;
        }

        private bool readInt3(out int[] i)
        {
            i = new int[3];
            bool ret = true;
            if (!readInt(out i[0]))
                ret = false;
            if (!readInt(out i[1]))
                ret = false;
            if (!readInt(out i[2]))
                ret = false;
            return ret;
        }

        private bool readFloat3(out float[] f)
        {
            f = new float[3];
            bool ret = true;
            if (!readFloat(out f[0]))
                ret = false;
            if (!readFloat(out f[1]))
                ret = false;
            if (!readFloat(out f[2]))
                ret = false;
            return ret;
        }

        private bool readVector3D(out Vector3D v)
        {
            v = new Vector3D();
            bool ret = true;
            if (!readFloat(out v.x))
                ret = false;
            if (!readFloat(out v.y))
                ret = false;
            if (!readFloat(out v.z))
                ret = false;
            return ret;
        }

        private bool readRGB(out ColorRGB c)
        {
            c = new ColorRGB();
            bool ret = true;
            if (!readFloat(out c.r))
                ret = false;
            if (!readFloat(out c.g))
                ret = false;
            if (!readFloat(out c.b))
                ret = false;
            return ret;
        }

        private bool readString(out string str)
        {
            if (scanner.yylex() != (int)TokenType.String)
            {
                Console.Error.WriteLine("Expected a string");
                str = "";
                return false;
            }
            int len = scanner.yytext.Length;
            str = scanner.yytext.Substring(1, len - 2);
            return true;
        }

        private bool readSymbol(out string sym)
        {
            if (scanner.yylex() != (int)TokenType.Symbol)
            {
                Console.Error.WriteLine("Expected a symbol");
                sym = "";
                return false;
            }
            int len = scanner.yytext.Length;
            sym = scanner.yytext.Substring(0, len + 1);
            //sym = scanner.yytext;
            return true;
        }

        private bool readIndex()
        {
            if (scanner.yylex() != (int)TokenType.Index)
            {
                Console.Error.WriteLine("Expected an index");
                return false;
            }
            return true;
        }

        private bool readIndex(out string ret)
        {
            if (scanner.yylex() != (int)TokenType.Index)
            {
                Console.Error.WriteLine("Expected an index");
                ret = "";
                return false;
            }
            ret = scanner.yytext.Replace(":", "");
            return true;
        }

        private bool readBlockStart()
        {
            if (scanner.yylex() != (int)TokenType.BlockStart)
            {
                Console.Error.WriteLine("Expected an open brace");
                return false;
            }
            return true;
        }

        private bool readBlockEnd()
        {
            if (scanner.yylex() != (int)TokenType.BlockEnd)
            {
                Console.Error.WriteLine("Expected a close brace");
                return false;
            }
            return true;
        }

        private int skip()
        {
            int nextToken = 0;
            int depth = 0;
            bool needNext = true;
            do
            {
                if (needNext) nextToken = scanner.yylex();
                else needNext = true;
                if (nextToken == (int)TokenType.BlockStart) depth++;
                if (nextToken == (int)TokenType.BlockEnd) if (depth > 0)
                    {
                        depth--;
                        nextToken = scanner.yylex();
                        needNext = false;
                    }
            } while (nextToken > (int)Tokens.EOF && (depth > 0 || (nextToken != (int)TokenType.Node && nextToken != (int)TokenType.BlockEnd)));
            return nextToken;
        }

        private bool loadScene(Scene scene)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if(nextToken != (int)TokenType.Node) {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                string text = scanner.yytext;
                if (text.Equals("*SCENE_FILENAME"))
                {
                    string blah;
                    readString(out blah);
                }
                else if (text.Equals("*SCENE_FIRSTFRAME"))
                {
                    if (!readInt(out scene.firstFrame)) success = false;
                }
                else if (text.Equals("*SCENE_LASTFRAME"))
                {
                    if (!readInt(out scene.lastFrame)) success = false;
                }
                else if (text.Equals("*SCENE_FRAMESPEED"))
                {
                    if (!readInt(out scene.frameSpeed)) success = false;
                }
                else if (text.Equals("*SCENE_TICKSPERFRAME"))
                {
                    if (!readInt(out scene.ticksPerFrame)) success = false;
                }
                else if (text.Equals("*SCENE_BACKGROUND_STATIC"))
                {
                    if (!readRGB(out scene.backgroundLight)) success = false;
                }
                else if (text.Equals("*SCENE_AMBIENT_STATIC"))
                {
                    if (!readRGB(out scene.ambientLight)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadMaterialMap(out MaterialMap map)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            map = new MaterialMap();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*MAP_NAME"))
                {
                    if (!readString(out map.name)) success = false;
                }
                else if (str.Equals("*MAP_CLASS"))
                {
                    if (!readString(out map.mapclass)) success = false;
                }
                else if (str.Equals("*MAP_SUBNO"))
                {
                    if (!readInt(out map.subno)) success = false;
                }
                else if (str.Equals("*MAP_AMOUNT"))
                {
                    if (!readFloat(out map.amount)) success = false;
                }
                else if (str.Equals("*BITMAP"))
                {
                    if (!readString(out map.imagePath)) success = false;
                }
                else if (str.Equals("*MAP_TYPE"))
                {
                    if (!readSymbol(out map.type)) success = false;
                }
                else if (str.Equals("*UVW_U_OFFSET"))
                {
                    if (!readFloat(out map.uOffset)) success = false;
                }
                else if (str.Equals("*UVW_V_OFFSET"))
                {
                    if (!readFloat(out map.vOffset)) success = false;
                }
                else if (str.Equals("*UVW_U_TILING"))
                {
                    if (!readFloat(out map.uTiling)) success = false;
                }
                else if (str.Equals("*UVW_V_TILING"))
                {
                    if (!readFloat(out map.vTiling)) success = false;
                }
                else if (str.Equals("*UVW_ANGLE"))
                {
                    if (!readFloat(out map.angle)) success = false;
                }
                else if (str.Equals("*UVW_BLUR"))
                {
                    if (!readFloat(out map.blur)) success = false;
                }
                else if (str.Equals("*UVW_BLUR_OFFSET"))
                {
                    if (!readFloat(out map.blurOffset)) success = false;
                }
                else if (str.Equals("*UVW_NOISE_AMT"))
                {
                    if (!readFloat(out map.noiseAmt)) success = false;
                }
                else if (str.Equals("*UVW_NOISE_SIZE"))
                {
                    if (!readFloat(out map.noiseSize)) success = false;
                }
                else if (str.Equals("*UVW_NOISE_LEVEL"))
                {
                    if (!readInt(out map.noiseLevel)) success = false;
                }
                else if (str.Equals("*UVW_NOISE_PHASE"))
                {
                    if (!readFloat(out map.noisePhase)) success = false;
                }
                else if (str.Equals("*BITMAP_FILTER"))
                {
                    if (!readSymbol(out map.bitmapFilter)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadMaterial(out Material material)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            int numSubMaterials = 0;

            material = new Material();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*MATERIAL_NAME"))
                {
                    if (!readString(out material.name)) success = false;
                }
                else if (str.Equals("*MATERIAL_CLASS"))
                {
                    if (!readString(out material.matclass)) success = false;
                }
                else if (str.Equals("*MATERIAL_AMBIENT"))
                {
                    if (!readRGB(out material.ambient)) success = false;
                }
                else if (str.Equals("*MATERIAL_DIFFUSE"))
                {
                    if (!readRGB(out material.diffuse)) success = false;
                }
                else if (str.Equals("*MATERIAL_SPECULAR"))
                {
                    if (!readRGB(out material.specular)) success = false;
                }
                else if (str.Equals("*MATERIAL_SHINE"))
                {
                    if (!readFloat(out material.shine)) success = false;
                }
                else if (str.Equals("*MATERIAL_SHINESTRENGTH"))
                {
                    if (!readFloat(out material.shineStrength)) success = false;
                }
                else if (str.Equals("*MATERIAL_TRANSPARENCY"))
                {
                    if (!readFloat(out material.transparency)) success = false;
                }
                else if (str.Equals("*MATERIAL_WIRESIZE"))
                {
                    if (!readFloat(out material.wiresize)) success = false;
                }
                else if (str.Equals("*MATERIAL_SHADING"))
                {
                    if (!readSymbol(out material.shading)) success = false;
                }
                else if (str.Equals("*MATERIAL_XP_FALLOFF"))
                {
                    if (!readFloat(out material.xpFalloff)) success = false;
                }
                else if (str.Equals("*MATERIAL_SELFILLUM"))
                {
                    if (!readFloat(out material.selfIllum)) success = false;
                }
                else if (str.Equals("*MATERIAL_FALLOFF"))
                {
                    if (!readSymbol(out material.falloff)) success = false;
                }
                else if (str.Equals("*MATERIAL_XP_TYPE"))
                {
                    if (!readSymbol(out material.xpType)) success = false;
                }
                else if (str.Equals("*MAP_DIFFUSE"))
                {
                    if (!loadMaterialMap(out material.diffuseMaterialMap)) success = false;
                }
                else if (str.Equals("*NUMSUBMTLS"))
                {
                    if (!readInt(out numSubMaterials)) success = false;
                }
                else if (str.Equals("*SUBMATERIAL"))
                {
                    int which;
                    if (!readInt(out which))
                    {
                        success = false;
                        continue;
                    }
                    if (which >= numSubMaterials)
                    {
                        Console.Error.WriteLine("File refered to non-existant submaterial");
                        continue;
                    }
                    if (material.subMaterials == null) material.subMaterials = new List<Material>();
                    Material subMaterial = new Material();
                    if (!loadMaterial(out subMaterial)) success = false;
                    material.subMaterials.Add(subMaterial);
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadMaterials(Scene scene)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            int materialCount;

            readBlockStart();

            if (scanner.yylex() != (int)TokenType.Node)
                Console.Error.WriteLine("Expected a node");
            if (!scanner.yytext.Equals("*MATERIAL_COUNT"))
                Console.Error.WriteLine("Expected a *MATERIAL_COUNT node");

            readInt(out materialCount);

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                if (scanner.yytext.Equals("*MATERIAL"))
                {
                    int m;
                    if (!readInt(out m))
                    {
                        success = false;
                        continue;
                    }
                    if (m >= materialCount)
                    {
                        Console.Error.WriteLine("File refered to non-existant submaterial");
                        continue;
                    }
                    Material material = new Material();
                    if (scene.materials == null) scene.materials = new List<Material>();
                    if (!loadMaterial(out material)) success = false;
                    scene.materials.Add(material);
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadTransform(out Transform trans)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            trans = new Transform();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*NODE_NAME"))
                {
                    if (!readString(out trans.name)) success = false;
                }
                else if (str.Equals("*INHERIT_POS"))
                {
                    if (!readInt3(out trans.inheritPos)) success = false;
                }
                else if (str.Equals("*INHERIT_ROT"))
                {
                    if (!readInt3(out trans.inheritRot)) success = false;
                }
                else if (str.Equals("*INHERIT_SCL"))
                {
                    if (!readInt3(out trans.inheritScl)) success = false;
                }
                else if (str.Equals("*TM_ROW0"))
                {
                    if (trans.matrix == null) trans.matrix = new float[3][];
                    if (!readFloat3(out trans.matrix[0])) success = false;
                }
                else if (str.Equals("*TM_ROW1"))
                {
                    if (trans.matrix == null) trans.matrix = new float[3][];
                    if (!readFloat3(out trans.matrix[1])) success = false;
                }
                else if (str.Equals("*TM_ROW2"))
                {
                    if (trans.matrix == null) trans.matrix = new float[3][];
                    if (!readFloat3(out trans.matrix[2])) success = false;
                }
                else if (str.Equals("*TM_POS"))
                {
                    if (!readVector3D(out trans.pos)) success = false;
                }
                else if (str.Equals("*TM_ROTAXIS"))
                {
                    if (!readVector3D(out trans.rotAxis)) success = false;
                }
                else if (str.Equals("*TM_ROTANGLE"))
                {
                    if (!readFloat(out trans.rotAngle)) success = false;
                }
                else if (str.Equals("*TM_SCALE"))
                {
                    if (!readVector3D(out trans.scale)) success = false;
                }
                else if (str.Equals("*TM_SCALEAXIS"))
                {
                    if (!readVector3D(out trans.scaleAxis)) success = false;
                }
                else if (str.Equals("*TM_SCALEAXISANG"))
                {
                    if (!readFloat(out trans.scaleAngle)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadMeshFaces(Mesh mesh)
        {
            bool success = true;
            int nextToken = 0;
            int f, i;
            bool haveNext = false;

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node" + " - " + scanner.yytext);
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*MESH_FACE"))
                {
                    string index;
                    if (!readIndex(out index))
                    {
                        success = false;
                        continue;
                    }
                    f = int.Parse(index);
                    if (f >= mesh.faceCount)
                        Console.Error.WriteLine("File referred to a non-existent face ({0} >= {1})", f, mesh.faces.Count);

                    Face face = mesh.faces[f];
                    face.vertex = new int[3];

                    for (i = 0; i < 3; i++)
                    {
                        if (!readIndex()) success = false;
                        if (!readInt(out face.vertex[i])) success = false;
                    }
                    if (!readIndex()) success = false;
                    if (!readInt(out face.ab)) success = false;
                    if (!readIndex()) success = false;
                    if (!readInt(out face.bc)) success = false;
                    if (!readIndex()) success = false;
                    if (!readInt(out face.ca)) success = false;
                    mesh.faces[f] = face;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadMeshNormals(Mesh mesh)
        {
            bool success = true;
            int nextToken = 0;
            int i;
            bool haveNext = false;

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                if (scanner.yytext.Equals("*MESH_FACENORMAL"))
                {
                    if (!readInt(out i))
                    {
                        success = false;
                        continue;
                    }
                    if (i >= mesh.faceCount)
                    {
                        Console.Error.WriteLine("File referred to a non-existent face");
                        success = false;
                        continue;
                    }

                    Face face = mesh.faces[i];
                    if (!readVector3D(out face.faceNormal)) success = false;
                    mesh.faces[i] = face;
                }
                else if (scanner.yytext.Equals("*MESH_VERTEXNORMAL"))
                {
                    if (!readInt(out i))
                    {
                        success = false;
                        continue;
                    }
                    if (i >= mesh.vertexCount)
                    {
                        Console.Error.WriteLine("File referred to a non-existent vertex");
                        success = false;
                        continue;
                    }

                    Vector3D normal = mesh.vertexNormals[i];
                    if (!readVector3D(out normal)) success = false;
                    mesh.vertexNormals[i] = normal;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadMesh(out Mesh mesh)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            mesh = new Mesh();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node" + " - " + scanner.yytext);
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*TIMEVALUE"))
                {
                    if (!readInt(out mesh.timeValue)) success = false;
                }
                else if (str.Equals("*MESH_NUMVERTEX"))
                {
                    if (!readInt(out mesh.vertexCount)) success = false;
                    mesh.vertexNormals = new List<Vector3D>();
                    for (int i = 0; i < mesh.vertexCount; i++) mesh.vertexNormals.Add(new Vector3D());
                }
                else if (str.Equals("*MESH_NUMFACES"))
                {
                    if (!readInt(out mesh.faceCount)) success = false;
                    mesh.faces = new List<Face>();
                    for (int i = 0; i < mesh.faceCount; i++) mesh.faces.Add(new Face());
                }
                else if (str.Equals("*MESH_VERTEX_LIST"))
                {
                    readBlockStart();
                    while ((nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
                    {
                        int i;
                        if (nextToken != (int)TokenType.Node || !scanner.yytext.Equals("*MESH_VERTEX"))
                        {
                            Console.Error.WriteLine("Expected a *MESH_VERTEX node");
                            continue;
                        }
                        if (!readInt(out i)) success = false;
                        if (i >= mesh.vertexCount)
                        {
                            Console.Error.WriteLine("File referred to a non-existent vertex");
                            continue;
                        }
                        if (mesh.verticies == null) mesh.verticies = new List<Vector3D>();
                        Vector3D vertex;
                        if (!readVector3D(out vertex)) success = false;
                        mesh.verticies.Add(vertex);
                    }
                }
                else if (str.Equals("*MESH_FACE_LIST"))
                {
                    if (!loadMeshFaces(mesh)) success = false;
                }
                else if (str.Equals("*MESH_NUMTVERTEX"))
                {
                    if (!readInt(out mesh.textureCoordinateCount)) success = false;
                }
                else if (str.Equals("*MESH_TVERTLIST"))
                {
                    readBlockStart();
                    while ((nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
                    {
                        int i;
                        if (nextToken != (int)TokenType.Node || !scanner.yytext.Equals("*MESH_TVERT"))
                        {
                            Console.Error.WriteLine("Expected a *MESH_TVERT node");
                            continue;
                        }
                        if (!readInt(out i)) success = false;
                        if (i >= mesh.textureCoordinateCount)
                        {
                            Console.Error.WriteLine("File referred to a non-existent vertex");
                            continue;
                        }
                        if (mesh.textureCoordinates == null) mesh.textureCoordinates = new List<Vector3D>();
                        Vector3D vertex;
                        if (!readVector3D(out vertex)) success = false;
                        mesh.textureCoordinates.Add(vertex);
                    }
                }
                else if (str.Equals("*MESH_NUMTVFACES"))
                {
                    int i;
                    if (!readInt(out i)) success = false;
                    if (i != mesh.faceCount)
                        Console.Error.WriteLine("Expected face texture count does not match face count");
                }
                else if (str.Equals("*MESH_TFACELIST"))
                {
                    readBlockStart();
                    while ((nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
                    {
                        int i;
                        if (nextToken != (int)TokenType.Node || !scanner.yytext.Equals("*MESH_TFACE"))
                        {
                            Console.Error.WriteLine("Expected a *MESH_TFACE node");
                            continue;
                        }
                        if (!readInt(out i)) success = false;
                        if (i >= mesh.faceCount)
                        {
                            Console.Error.WriteLine("File referred to a non-existent face");
                            continue;
                        }
                        if (mesh.faces != null)
                        {
                            Face face = mesh.faces[i];
                            if (!readInt3(out face.textureCoordinates)) success = false;
                            mesh.faces[i] = face;
                        }
                        else
                        {
                            success = false;
                        }
                    }
                }
                else if (str.Equals("*MESH_NORMALS"))
                {
                    if (!loadMeshNormals(mesh)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadGeomObject(Scene scene)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            GeomObject obj = new GeomObject();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node" + " - " + scanner.yytext);
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*NODE_NAME"))
                {
                    if (!readString(out obj.name)) success = false;
                }
                else if (str.Equals("*NODE_TM"))
                {
                    if (!loadTransform(out obj.transform)) success = false;
                }
                else if (str.Equals("*MESH"))
                {
                    if (!loadMesh(out obj.mesh)) success = false;
                }
                else if (str.Equals("*PROP_MOTIONBLUR"))
                {
                    if (!readBool(out obj.motionBlur)) success = false;
                }
                else if (str.Equals("*PROP_CASTSHADOW"))
                {
                    if (!readBool(out obj.castShadow)) success = false;
                }
                else if (str.Equals("*PROP_RECVSHADOW"))
                {
                    if (!readBool(out obj.recieveShadow)) success = false;
                }
                else if (str.Equals("*TM_ANIMATION"))
                {
                    skip();
                }
                else if (str.Equals("*MATERIAL_REF"))
                {
                    if (!readInt(out obj.material)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            scene.objs.Add(obj);
            return success;
        }

        private bool loadLightSettings(out LightSettings settings)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            settings = new LightSettings();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node" + " - " + scanner.yytext);
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*TIMEVALUE"))
                {
                    if (!readInt(out settings.timeValue)) success = false;
                }
                else if (str.Equals("*LIGHT_COLOR"))
                {
                    if (!readRGB(out settings.color)) success = false;
                }
                else if (str.Equals("*LIGHT_INTENS"))
                {
                    if (!readFloat(out settings.intensity)) success = false;
                }
                else if (str.Equals("*LIGHT_ASPECT"))
                {
                    if (!readFloat(out settings.aspect)) success = false;
                }
                else if (str.Equals("*LIGHT_TDIST"))
                {
                    if (!readFloat(out settings.tdist)) success = false;
                }
                else if (str.Equals("*LIGHT_MAPBIAS"))
                {
                    if (!readFloat(out settings.mapBias)) success = false;
                }
                else if (str.Equals("*LIGHT_MAPRANGE"))
                {
                    if (!readFloat(out settings.mapRange)) success = false;
                }
                else if (str.Equals("*LIGHT_MAPSIZE"))
                {
                    if (!readInt(out settings.mapSize)) success = false;
                }
                else if (str.Equals("*LIGHT_RAYBIAS"))
                {
                    if (!readFloat(out settings.rayBias)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadLight(Scene scene)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            Light light = new Light();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*NODE_NAME"))
                {
                    if (!readString(out light.name)) success = false;
                }
                else if (str.Equals("*LIGHT_TYPE"))
                {
                    if (!readSymbol(out light.type)) success = false;
                }
                else if (str.Equals("*NODE_TM"))
                {
                    if (!loadTransform(out light.transform)) success = false;
                }
                else if (str.Equals("*LIGHT_SHADOWS"))
                {
                    if (!readSymbol(out light.shadows)) success = false;
                }
                else if (str.Equals("*LIGHT_USELIGHT"))
                {
                    if (!readBool(out light.useLight)) success = false;
                }
                else if (str.Equals("*LIGHT_SPOTSHAPE"))
                {
                    if (!readSymbol(out light.spotShape)) success = false;
                }
                else if (str.Equals("*LIGHT_USEGLOBAL"))
                {
                    if (!readBool(out light.useGlobal)) success = false;
                }
                else if (str.Equals("*LIGHT_ABSMAPBIAS"))
                {
                    if (!readInt(out light.absMapBias)) success = false;
                }
                else if (str.Equals("*LIGHT_OVERSHOOT"))
                {
                    if (!readInt(out light.overshoot)) success = false;
                }
                else if (str.Equals("*LIGHT_SETTINGS"))
                {
                    if (!loadLightSettings(out light.settings)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            if (scene.lights == null) scene.lights = new List<Light>();
            scene.lights.Add(light);
            return success;
        }

        private bool loadCameraSettings(out CameraSettings settings)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            settings = new CameraSettings();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*TIMEVALUE"))
                {
                    if (!readInt(out settings.timeValue)) success = false;
                }
                else if (str.Equals("*CAMERA_NEAR"))
                {
                    if (!readFloat(out settings.nearVal)) success = false;
                }
                else if (str.Equals("*CAMERA_FAR"))
                {
                    if (!readFloat(out settings.farVal)) success = false;
                }
                else if (str.Equals("*CAMERA_FOV"))
                {
                    if (!readFloat(out settings.fov)) success = false;
                }
                else if (str.Equals("*CAMERA_TDIST"))
                {
                    if (!readFloat(out settings.tdist)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            return success;
        }

        private bool loadCamera(Scene scene)
        {
            bool success = true;
            int nextToken = 0;
            bool haveNext = false;

            Camera camera = new Camera();

            readBlockStart();

            while (haveNext || (nextToken = scanner.yylex()) != (int)TokenType.BlockEnd)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }
                haveNext = false;

                string str = scanner.yytext;
                if (str.Equals("*NODE_NAME"))
                {
                    if (!readString(out camera.name)) success = false;
                }
                else if (str.Equals("*CAMERA_TYPE"))
                {
                    if (!readSymbol(out camera.type)) success = false;
                    if (camera.type.Equals("Target"))
                    {
                        scanner.yylex();
                        if (!loadTransform(out camera.camera)) success = false;
                        scanner.yylex();
                        if (!loadTransform(out camera.target)) success = false;
                    }
                }
                else if (str.Equals("*CAMERA_SETTINGS"))
                {
                    if (!loadCameraSettings(out camera.settings)) success = false;
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd || nextToken < 1) break;
                    haveNext = true;
                }
            }
            if (scene.cameras == null) scene.cameras = new List<Camera>();
            scene.cameras.Add(camera);
            return success;
        }

        private Scene load()
        {
            int version;
            int nextToken;
            bool haveNext = true;

            Scene scene = new Scene();

            if (scanner.yylex() != (int)TokenType.Node)
            {
                Console.Error.WriteLine("Expected a node");
            }
            if (!scanner.yytext.Equals("*3DSMAX_ASCIIEXPORT"))
            {
                Console.Error.WriteLine("File must start with *3DSMAX_ASCIIEXPORT");
            }

            readInt(out version);
            if (version != 200)
            {
                Console.Error.WriteLine("Warning: loader has not been tested with this file version {%i}", version);
            }

            scene.materials = new List<Material>();
            scene.objs = new List<GeomObject>();
            scene.lights = new List<Light>();
            scene.cameras = new List<Camera>();

            while ((nextToken = scanner.yylex()) > (int)Tokens.EOF)
            {
                if (nextToken != (int)TokenType.Node)
                {
                    Console.Error.WriteLine("Expected a node");
                }

                string text = scanner.yytext;
                if (text.Equals("*SCENE"))
                {
                    loadScene(scene);
                }
                else if (text.Equals("*MATERIAL_LIST"))
                {
                    loadMaterials(scene);
                }
                else if (text.Equals("*GEOMOBJECT"))
                {
                    loadGeomObject(scene);
                }
                else if (text.Equals("*LIGHTOBJECT"))
                {
                    loadLight(scene);
                }
                else if (text.Equals("*CAMERAOBJECT"))
                {
                    loadCamera(scene);
                }
                else if (text.Equals("*COMMENT"))
                {
                    string str;
                    readString(out str);
                }
                else
                {
                    if ((nextToken = skip()) == (int)TokenType.BlockEnd) break;
                    haveNext = true;
                }
            }

            return scene;
        }

        public Scene loadFilename(string path)
        {
            string contents = File.ReadAllText(path);
            scanner = new Scanner();
            scanner.SetSource(contents, 0);
            return load();
        }

        public Scene loadString(string contents)
        {
            scanner = new Scanner();
            scanner.SetSource(contents, 0);
            return load();
        }
    }
}