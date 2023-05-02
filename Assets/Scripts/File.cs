using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace Figma
{
    public class File
    {
        public Document document;
    }

    public class Document
    {
        public Page[] children;
    }

    public class Page
    {
        public Layer[] children;
    }

    public class Layer
    {
        public Type type;
        public string name;
        public Box absoluteBoundingBox;
        public Layer[] children;
    }

    public enum Type
    {
        DOCUMENT,
        CANVAS,
        FRAME,
        RECTANGLE
    }

    public class Box
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

}
