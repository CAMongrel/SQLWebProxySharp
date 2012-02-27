using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SQLWebProxySharpEntities.Types
{
    internal class TypeSerializer
    {
        public static string Serialize(object value)
        {
            string valueType = "null";
            if (value != null)
                valueType = value.GetType().FullName;

            string valueValue = "";
            switch (valueType)
            {
                case "System.Decimal":
                    valueValue = SerializeDecimal((decimal)value);
                    break;

                case "System.Double":
                    valueValue = SerializeDouble((double)value);
                    break;

                case "System.Single":
                    valueValue = SerializeSingle((float)value);
                    break;

                case "System.String":
                    valueValue = value as string;
                    break;

                case "System.Boolean":
                    valueValue = SerializeBoolean((bool)value);
                    break;

                case "System.Byte":
                    valueValue = SerializeByte((byte)value);
                    break;

                case "System.SByte":
                    valueValue = SerializeSByte((sbyte)value);
                    break;

                case "System.Int16":
                    valueValue = SerializeInt16((short)value);
                    break;

                case "System.UInt16":
                    valueValue = SerializeUInt16((ushort)value);
                    break;

                case "System.Int32":
                    valueValue = SerializeInt32((int)value);
                    break;

                case "System.UInt32":
                    valueValue = SerializeUInt32((uint)value);
                    break;
            }

            string result = "<Value Type=\"" + valueType + "\">" + valueValue + "</Value>";
            return result;
        }

        public static object Deserialize(XmlNode node)
        {
            if (node.Name != "Value")
                return null;

            string type = node.Attributes["Type"].InnerText;
            string value = node.InnerText;

            switch (type)
            {
                default:
                    return null;

                case "System.Decimal":
                    return DeserializeDecimal(value);

                case "System.Double":
                    return DeserializeDouble(value);

                case "System.Single":
                    return DeserializeSingle(value);

                case "System.String":
                    return value;

                case "System.Boolean":
                    return DeserializeBoolean(value);

                case "System.Byte":
                    return DeserializeByte(value);

                case "System.SByte":
                    return DeserializeSByte(value);

                case "System.Int16":
                    return DeserializeInt16(value);

                case "System.UInt16":
                    return DeserializeUInt16(value);

                case "System.Int32":
                    return DeserializeInt32(value);

                case "System.UInt32":
                    return DeserializeUInt32(value);
            }
        }

        #region Serialize
        private static string SerializeDecimal(decimal value)
        {
            return value.ToString();
        }

        private static string SerializeBoolean(bool value)
        {
            return value.ToString();
        }

        private static string SerializeByte(byte value)
        {
            return value.ToString();
        }

        private static string SerializeSByte(sbyte value)
        {
            return value.ToString();
        }

        private static string SerializeInt16(short value)
        {
            return value.ToString();
        }

        private static string SerializeUInt16(ushort value)
        {
            return value.ToString();
        }

        private static string SerializeInt32(int value)
        {
            return value.ToString();
        }

        private static string SerializeUInt32(uint value)
        {
            return value.ToString();
        }

        private static string SerializeSingle(float value)
        {
            return value.ToString();
        }

        private static string SerializeDouble(double value)
        {
            return value.ToString();
        }
        #endregion

        #region Deserialize
        private static decimal DeserializeDecimal(string value)
        {
            decimal result = 0;
            if (decimal.TryParse(value, out result))
                return result;
            return 0;
        }

        private static bool DeserializeBoolean(string value)
        {
            bool result = false;
            if (bool.TryParse(value, out result))
                return result;
            return false;
        }

        private static byte DeserializeByte(string value)
        {
            byte result = 0;
            if (byte.TryParse(value, out result))
                return result;
            return 0;
        }

        private static sbyte DeserializeSByte(string value)
        {
            sbyte result = 0;
            if (sbyte.TryParse(value, out result))
                return result;
            return 0;
        }

        private static short DeserializeInt16(string value)
        {
            short result = 0;
            if (short.TryParse(value, out result))
                return result;
            return 0;
        }

        private static ushort DeserializeUInt16(string value)
        {
            ushort result = 0;
            if (ushort.TryParse(value, out result))
                return result;
            return 0;
        }

        private static int DeserializeInt32(string value)
        {
            int result = 0;
            if (int.TryParse(value, out result))
                return result;
            return 0;
        }

        private static uint DeserializeUInt32(string value)
        {
            uint result = 0;
            if (uint.TryParse(value, out result))
                return result;
            return 0;
        }

        private static float DeserializeSingle(string value)
        {
            float result = 0;
            if (float.TryParse(value, out result))
                return result;
            return 0;
        }

        private static double DeserializeDouble(string value)
        {
            double result = 0;
            if (double.TryParse(value, out result))
                return result;
            return 0;
        }
        #endregion
    }
}
