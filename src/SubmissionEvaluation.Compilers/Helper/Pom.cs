using System.Xml.Linq;

namespace SubmissionEvaluation.Compilers.Helper
{
    internal class Pom
    {
        protected XDocument document;
        protected XNamespace ns;

        public Pom(string content)
        {
            document = XDocument.Parse(content);
            ns = document.Root.Attribute("xmlns").Value;
        }

        public string ArtifactId { get => GetArtifactId(); set => SetArtifactId(value); }

        public string Version { get => GetVersion(); set => SetVersion(value); }

        public string BuildDirectory { get => GetBuildDirectory(); set => SetBuildDirectory(value); }

        protected XElement GetArtifactIdNode()
        {
            var artifactIdNode = document.Root.Element(ns + "artifactId");
            return artifactIdNode;
        }

        protected void SetArtifactId(string artifactId)
        {
            var node = GetArtifactIdNode();
            node.Value = artifactId;
        }

        protected string GetArtifactId()
        {
            var node = GetArtifactIdNode();
            return node.Value;
        }

        protected XElement GetBuildDirectoryNode()
        {
            var buildNode = document.Root.Element(ns + "build");
            var directoryNode = buildNode?.Element(ns + "directory");
            return directoryNode;
        }

        protected XElement CreateBuildDirectoryNode()
        {
            var buildNode = document.Root.Element(ns + "build");
            if (buildNode == null)
            {
                buildNode = new XElement(ns + "build");
                document.Root.Add(buildNode);
            }

            var directoryNode = buildNode.Element(ns + "directory");
            if (directoryNode == null)
            {
                directoryNode = new XElement(ns + "directory");
                buildNode.Add(directoryNode);
            }

            return directoryNode;
        }

        protected void SetBuildDirectory(string buildDirectory)
        {
            var node = GetBuildDirectoryNode();
            if (node == null)
            {
                node = CreateBuildDirectoryNode();
            }

            node.Value = buildDirectory;
        }

        protected string GetBuildDirectory()
        {
            var node = GetBuildDirectoryNode();
            return node.Value;
        }

        protected XElement GetVersionNode()
        {
            var versionNode = document.Root.Element(ns + "version");
            return versionNode;
        }

        protected void SetVersion(string version)
        {
            var versionNode = GetVersionNode();
            versionNode.Value = version;
        }

        protected string GetVersion()
        {
            var versionNode = GetVersionNode();
            return versionNode.Value;
        }

        public string GetXMLString()
        {
            return document.ToString();
        }

        public static Pom GetDefaultPom(string mainClass)
        {
            return new Pom($@"<project xmlns=""http://maven.apache.org/POM/4.0.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
  xsi:schemaLocation=""http://maven.apache.org/POM/4.0.0 http://maven.apache.org/maven-v4_0_0.xsd"">
  <modelVersion>4.0.0</modelVersion>
  <groupId>test</groupId>
  <artifactId>build</artifactId>
  <packaging>jar</packaging>
  <version>1.0</version>
  <name>build</name>
  <build>
    <directory>../bin</directory>
      <plugins>
        <plugin>
          <groupId>org.apache.maven.plugins</groupId>
          <artifactId>maven-jar-plugin</artifactId>
          <version>3.1.0</version>
          <configuration>
            <archive>
              <manifest>
                <addClasspath>true</addClasspath>
                <mainClass>{mainClass}</mainClass>
              </manifest>
            </archive>
          </configuration>
          </plugin>
      </plugins>
  </build>
  <properties>
    <java.version>11</java.version>
    <maven.compiler.source>${{java.version}}</maven.compiler.source>
    <maven.compiler.target>${{java.version}}</maven.compiler.target>
    <project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
  </properties>
</project>");
        }
    }
}
