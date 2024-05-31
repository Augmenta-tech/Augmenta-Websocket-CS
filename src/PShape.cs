using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Augmenta
{
    public class PShape<T> : PContainer<T> where T : struct
    {
        public Shape<T> shape;
        public PShape(BasePleiadesClient client, JSONObject o, PContainer<T> parent, ContainerType type) : base(client, o, parent, type)
        {
            setupShape(o["shape"]);
            UnityEngine.Debug.Log("PShape " + o.ToString());

        }

        protected override void handleParamUpdateInternal(string prop, JSONObject data)
        {
            base.handleParamUpdateInternal(prop, data);
            if (prop == "shape") setupShape(data["shape"]);
            else if (prop == "shapeParam")
            {
                if (shape != null)
                {
                    foreach (var p in data.keys)
                    {
                        shape.handleParamUpdate(p, data[p]);
                    }
                }
            }
        }

        void setupShape(JSONObject o)
        {
            UnityEngine.Debug.Log("Setup shape " + o.ToString());
            if (shape != null)
                shape = null;

            if (o == null)
                return;

            switch (o["type"].str)
            {
                case "Sphere":
                    shape = new SphereShape<T>(o);
                    break;

                case "Box":
                    shape = new BoxShape<T>(o);
                    break;

                case "Cylinder":
                    shape = new CylinderShape<T>(o);
                    break;

                case "Cone":
                    shape = new ConeShape<T>(o);
                    break;

                case "Mesh":
                    shape = new MeshShape<T>(o);
                    break;
            }

        }
    }

    public class Shape<T> where T : struct
    {
        public enum ShapeType { Sphere, Box, Cylinder, Cone, Plane, Mesh }
        public ShapeType shapeType;

        public Shape(JSONObject o, ShapeType st)
        {
            shapeType = st;
            UnityEngine.Debug.Log("Shape " + o.ToString());
        }

        virtual public void handleParamUpdate(string prop, JSONObject data) { }
    }

    public class SphereShape<T> : Shape<T> where T : struct
    {
        public float radius;

        public SphereShape(JSONObject o) : base(o, ShapeType.Sphere)
        {
            radius = o["radius"].f;
        }

        public override void handleParamUpdate(string prop, JSONObject data)
        {
            base.handleParamUpdate(prop, data);
            if (prop == "radius") radius = data.f;
        }

    }

    public class BoxShape<T> : Shape<T> where T : struct
    {
        public T size;
        public BoxShape(JSONObject o) : base(o, ShapeType.Box)
        {
            size = Utils.GetVector<T>(o["boxSize"]);
        }

        public override void handleParamUpdate(string prop, JSONObject data)
        {
            base.handleParamUpdate(prop, data);
            if (prop == "boxSize") size = Utils.GetVector<T>(data);
        }
    }

    public class CylinderShape<T> : Shape<T> where T : struct
    {
        public float radius;
        public float height;
        public CylinderShape(JSONObject o) : base(o, ShapeType.Cylinder)
        {
            radius = o["radius"].f;
            height = o["height"].f;
        }

        public override void handleParamUpdate(string prop, JSONObject data)
        {
            base.handleParamUpdate(prop, data);
            if (prop == "radius") radius = data.f;
            else if (prop == "height") height = data.f;
        }
    }

    public class ConeShape<T> : Shape<T> where T : struct
    {
        public float radius;
        public float height;

        public ConeShape(JSONObject o) : base(o, ShapeType.Cone)
        {
            radius = o["radius"].f;
            height = o["height"].f;
        }

        public override void handleParamUpdate(string prop, JSONObject data)
        {
            base.handleParamUpdate(prop, data);
            if (prop == "radius") radius = data.f;
            else if (prop == "height") height = data.f;
        }
    }

    public class MeshShape<T> : Shape<T> where T : struct
    {
        public List<T> points;
        public MeshShape(JSONObject o) : base(o, ShapeType.Mesh)
        {
            points = new List<T>();
            var p = o["points"];
            for (int i = 0; i < p.Count; i++)
                points.Add(Utils.GetVector<T>(p[i]));
        }

        public override void handleParamUpdate(string prop, JSONObject data)
        {
            base.handleParamUpdate(prop, data);
            if (prop == "points")
            {
                points.Clear();
                for (int i = 0; i < data.Count; i++)
                    points.Add(Utils.GetVector<T>(data[i]));
            }
        }
    }
}