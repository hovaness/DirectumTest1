using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;
using Formatting = Newtonsoft.Json.Formatting;


class Program
{
    static void Main(String[] args)
    {
        string jsonPath = "input.json";
        string xmlPath = "config.xml";
        JsonParser(jsonPath,xmlPath);
    }

    static void JsonParser(string jsonPath, string configPath)
    {
        if(string.IsNullOrEmpty(jsonPath) || string.IsNullOrEmpty(configPath))
        {
            Console.WriteLine("Пути пустые");
            return;
        }
        try
        {
            string jsonData = File.ReadAllText(jsonPath);
            JObject jsonSource = JObject.Parse(jsonData);

            XDocument config = XDocument.Load(configPath);
            XElement configRoot = config.Root;

            JObject resultObject = new JObject();

            foreach(XElement property in configRoot.Element("properties").Elements("property"))
            {
                string propertyName = property.Attribute("name").Value;
                string propertyType = property.Attribute("type").Value;
                string sourceProperty = property.Attribute("source").Value;

                JToken propertyValue = jsonSource[sourceProperty];

                if (propertyType == "integer")
                    resultObject[propertyName] = propertyValue.ToObject<int>();

                else if (propertyType == "string")
                    resultObject[propertyName] = propertyValue.ToString();

                else
                    throw new Exception($"Неправильный тип: {propertyType}");
            }
            string resultJson = resultObject.ToString(Formatting.Indented);
            string resultFile = configRoot.Attribute("name").Value + ".json";
            File.WriteAllText(resultFile, resultJson);
            Console.WriteLine($"Файл успешно записался под названием {resultFile}");
        }
        catch (FileNotFoundException exception)
        {
            Console.WriteLine("Файл не был найден: " + exception.Message);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Произошла ошибка: " + exception.Message);
        }
    }

  
}