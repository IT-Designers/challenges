using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubmissionEvaluation.Shared.Models.Challenge;
using SubmissionEvaluation.Shared.Models.Shared;
using SubmissionEvaluation.Shared.Models.Test;

namespace SubmissionEvaluation.Client.Services
{
    /**
     * Due to the complexity of the file system here follows a detailed documentation of the behaviour.
     * How the file system is worked up:
     * Case A: 
     * A user creates a new test.
     * Create.razor:
     * Test is assigned a new Id (the lowest free) that is used to identify the files that belong to this specific test.
     * Method "CreateFoldersForTest" is called and returned TestFolder (Item 1 of Tupel) assigned to parameter folder.
     * 
     * Case B:
     * A user edits an existing test. 
     * Edit.razor:
     * Fetches the ChallengeModel and the TestModel from the server, both containing a flat list of files. Fileexplorer structure 
     * is represented so far by the OriginalName of the files. In the TestModel the Name represents the file it is compiled to for tests. 
     * Not differing for new Tests. Path is empty yet.
     * The method "ReloadingFolderStructure" is called with the ChallengeModel and TestModel as parameters. Return is the Testfolder. 
     * For further detail see method description.
     * 
     * ALWAYS:
     * The inputfiles of the test are assigned to FlatFiles. TODO: Find out where it is used for.
     */
    public class FileExplorerDomain
    {
        public delegate void DomainChanged();

        public string Path { get; set; }
        public List<File> Files { get; set; }
        public List<DetailedInputFile> NewFiles { get; set; }
        public List<File> FlatFiles { get; set; }
        public bool InFileEdit { get; set; } = false;
        public File SelectedFile { get; set; } = new File();
        public event DomainChanged DomainEvent;

        public void InvokeEvent()
        {
            DomainEvent?.Invoke();
        }

        public void OpenFolderZone(Folder folder)
        {
            //Path is missing first segment, cause this is the tests index and not supposed to be seen by the user. 
            var Path = (folder.Path + folder.Name + Folder.pathSeperator).Substring(folder.Path.Split(Folder.pathSeperator.ToCharArray())[0].Length);
            Path = Path.Replace(Folder.pathSeperator, "/");
            this.Path = Path;
            Files = folder.FilesInFolder;
            NewFiles = folder.NewFilesInFolder;
            InvokeEvent();
        }

        /**
         * Turns the hierarchy of folders to a flat list, that can be sent to the server, making Name and OriginalName to the full name including the PATH. Recursive.
         */
        public List<DetailedInputFile> ExtractFoldersToServerFiles(List<DetailedInputFile> folderContent)
        {
            var list = new List<DetailedInputFile>();
            foreach (var file in folderContent)
            {
                if (file is Folder folder)
                {
                    var newList = ExtractFoldersToServerFiles(folder.NewFilesInFolder);
                    foreach (var newFile in newList)
                    {
                        list.Add(newFile);
                    }

                    file.Name = file.Path + file.Name + Folder.pathSeperator;
                    file.OriginalName = file.Path + file.OriginalName + Folder.pathSeperator;
                }
                else
                {
                    file.Name = file.Path + file.Name;
                    file.OriginalName = file.Path + file.OriginalName;
                }

                list.Add(file);
            }

            return list;
        }

        /**
         * Updates:
         * Files or folders that were renamed, renaming all paths of subfiles too.
         * Files that were hidden or unhidden / deleted.
         * Files thats last modified date was changed.
         */
        public List<T> UpdateAllChanges<T>(List<File> update, List<T> toBeUpdated) where T : File
        {
            update.ForEach(x =>
            {
                if (x.Path.StartsWith("_"))
                {
                    foreach (File replaceable in toBeUpdated)
                    {
                        replaceable.Name = replaceable.Name.Replace(x.Path.Substring(1, x.Path.Length - 2) + x.OriginalName, x.Path + x.Name);
                    }
                }

                if (x.Name != x.OriginalName)
                    //TODO: Find a better algorithm for this. Not that important due suggesting it´s never gonna be more than 100 files in practice anyway.
                {
                    foreach (File replaceable in toBeUpdated)
                    {
                        replaceable.Name = replaceable.Name.Replace(x.Path + x.OriginalName, x.Path + x.Name);
                    }
                }

                if (x.IsFolder)
                {
                    ((Folder) x).FilesInFolder.ForEach(y => y.Path = y.Path.Replace(x.OriginalName, x.Name));
                    //To be updated is supposed to be a flat list being synchronized with the changes in the non-flat projection.
                    UpdateAllChanges(((Folder) x).FilesInFolder, toBeUpdated);
                }

                if (x.IsDelete)
                {
                    toBeUpdated.Where(y => y.Name.StartsWith(x.Path + x.Name) || y.Name.StartsWith(x.Path + x.Name + Folder.pathSeperator)).ToList()
                        .ForEach(y => y.IsDelete = true);
                }

                if (x.LastModified != null)
                {
                    toBeUpdated.Where(y => y.Name.Equals(x.Path + x.Name) || y.Name.Equals(x.Path + x.Name + Folder.pathSeperator)).ToList()
                        .ForEach(y => y.LastModified = x.LastModified);
                }
            });
            return toBeUpdated;
        }

        //n is the layer you´re in. Start with 1.
        //Changes name only to name of the file without path and moves the path into PATH parameter. 
        //Note: If migrated test, the PATH IS STILL MISSING the testId/Input/ part. Being added in "CheckForDeeperFiles" and "CheckIfMultipleUsedFileOrFolder"
        public List<File> ConvertToFolderStructure<T>(List<T> ToBeConverted, int n) where T : File
        {
            ToBeConverted.ForEach(x =>
            {
                var splitted = x.Name.Split(Folder.pathSeperator.ToCharArray());
            });
            var topLevel = ToBeConverted.Select(x => x).Where(x =>
            {
                var splitted = x.Name.Split(Folder.pathSeperator.ToCharArray());
                return splitted.Count() == n || splitted.Count() == n + 1 && splitted[n].Equals(string.Empty);
            });
            var result = new List<File>();
            foreach (File file in topLevel)
            {
                var splitted = file.Name.Split(Folder.pathSeperator.ToCharArray());
                if (file.Name.EndsWith(Folder.pathSeperator))
                {
                    var folder = new Folder
                    {
                        IsFolder = true,
                        Name = splitted[splitted.Length - 2],
                        OriginalName = splitted[splitted.Length - 2],
                        IsDelete = file.IsDelete,
                        IsExpanded = false,
                        //This assigns as the path the full name of the file minus the real name of the file. 
                        Path = file.Name.Substring(0, file.Name.Length - 1 - splitted[splitted.Length - 2].Length)
                    };
                    folder.FilesInFolder =
                        ConvertToFolderStructure(
                            ToBeConverted.Select(x => x).Where(y =>
                                y.Name.StartsWith(file.Name) & !y.Equals(file) || y.Name.StartsWith("_" + file.Name) & !y.Equals(file)).ToList(), n + 1);
                    result.Add(folder);
                }
                else
                {
                    var added = new File
                    {
                        Path = file.Name.Substring(0, file.Name.Length - splitted[splitted.Length - 1].Length),
                        Name = splitted[splitted.Length - 1],
                        OriginalName = splitted[splitted.Length - 1]
                    };
                    result.Add(added);
                }
            }

            return result;
        }

        /**
         * Case A: 
         * The test inputted is a test, that was created after introducing file explorer. 
         * Calls ConvertToFolderStructure on all Files of the challengeModel, that start with the tests Id. 
         * The return is the top lvl folder with:
         * Name: {testId}
         * OriginalName: {testId}
         * Path: string.Empty
         * FilesInFolder: 
         * |- Input (path = {stringId}/, FilesInFolder = allFiles,that were created in FileExplorer in folder structure, WITH PATH)
         * 
         * Case B: 
         * The test inputted is a test that was created before introducing the file explorer.
         * The method "CreateFoldersForTest" is called, returning the necessary folders for file explorer structure WITH PATH.
         * The method "MigrateOldTests" is called and given as parameters the challengeModel, the challengeTest and the "Input"-Folder, 
         * that is inside Folder.NewFilesInFolder (Notice: Differs from above case, where it would be in FilesInFolder, relevant, when sending to server.)
         * 
         * ALWAYS: 
         * Returns the test folder, which contains the folder "Input".
         */
        public Folder ReloadingFolderStructure(ExtendedChallengeModel challengeModel, ChallengeTest challengeTest)
        {
            Folder folder;
            try
            {
                //The input to this method is actually a list containing all subfolders of the testfolder and the testfolder itself, so it´s return toplevel domain is only the folder itself.
                folder = (Folder) ConvertToFolderStructure(
                    challengeModel.Files.Where(x =>
                            x.Name.StartsWith(challengeTest.Id + Folder.pathSeperator) || x.Name.StartsWith("_" + challengeTest.Id + Folder.pathSeperator))
                        .ToList(), 1).First(x => x is Folder);
            }
            catch (InvalidOperationException)
            {
                var (item1, item2) = CreateFoldersForTest(challengeModel, challengeTest);
                folder = item1;
                MigrateOldTests(challengeTest, challengeModel, item2);
            }

            return folder;
        }

        /**
         * A folder for the test is created, containing an input folder for the input files, hierarchy is:
         *  
         * {testId}
         * |- Input(Path is already assigned as {testId}/ )
         * 
         * Remark: Input is in {testId}.NewFilesInFolder, important when extracting it for server.
         * Returns the testfolder and the inputfolder inside it.
         */
        public (Folder, Folder) CreateFoldersForTest(ExtendedChallengeModel challengeModel, ChallengeTest challengeTest)
        {
            var folder = new Folder {Name = challengeTest.Id.ToString(), OriginalName = challengeTest.Id.ToString(), IsExpanded = false, Path = string.Empty};
            var inputFolder = new Folder {Name = "Input", OriginalName = "Input", IsExpanded = false, Path = folder.Path + folder.Name + Folder.pathSeperator};
            folder.NewFilesInFolder.Add(inputFolder);
            //Now added test-folder to challengemodel.
            challengeModel.NewFiles.Add(folder);
            return (folder, inputFolder);
        }

        public Folder FetchFolder(string wish, Folder testFolder)
        {
            try
            {
                if (testFolder.FilesInFolder.Count != 0)
                {
                    var fetchedFolder = (Folder) testFolder.FilesInFolder.Where(x => x.Name == wish && x.IsFolder).First();
                    return fetchedFolder;
                }

                return (Folder) testFolder.NewFilesInFolder.Where(x => x.Name == wish && x.IsFolder).First();
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
        }

        /**
         * Adds a inputfile in the TestModel for every file in the file explorer, that is not only used for holding the files.
         */
        public void MapFilesToTest<T>(List<T> oldFiles, List<File> toBeMapped, string Id) where T : File
        {
            oldFiles.Where(x => (x.Name.StartsWith(Id) || x.Name.StartsWith("_" + Id)) & !FolderHolder(x.Name, Id)).ToList().ForEach(x => toBeMapped.Add(
                new File {Name = x.Name.Replace(Folder.pathSeperator, "/"), OriginalName = x.Name, LastModified = x.LastModified, IsDelete = x.IsDelete}));
        }

        /**
         * Is used to migrate tests, that were created before file explorer existed. 
         * Case A: 
         * Every test accesses different files or it is the first test of this challenge edited.
         * 
         * Case B: 
         * Multiple tests access the same files and it is not the first test of the challenge edited.
         * 
         * Procedure:
         * The folder seperator of the tests inputfiles is replaced by the internally used folder seperator.
         * Inputfiles are copied into another list, that is used to add additional files needed since migration.
         * Iterates over all InputFiles and checks, if any of them is inside a folder, if so, calls "CreateNeededFoldersForOld", 
         * which creates and adds these folders to the other list WITHOUT PATH. (Name is full path).
         * Checks if any file is already used by another test, if so, adds it´s original name to a list of multipleUsedFileNames.
         * Calls method "ConvertToFolderStructure" with the other list as parameter, which returns this list converted WITH PATH (without {testId}/Input).
         * Adds all folders to InputFolder.NewFilesInFolder, because they otherwise would be forgotten when sending it to the server.
         * Adds all files to InputFolder.FilesInFolder to make them visible. Already existing at the server, so no NewFiles.
         * Calls method "CheckIfMultipleFileOrFolder" which searches the multiple used files and folders in the "FilesInFolder" - list and moves them to 
         * the "NewFilesInFolder" - list recursively, to mark them for server.
         * Calls method "CheckForDeeperFiles" with the challengeModel, the now to folder structure converted other list and the Id of the test, 
         * which adds the {testId}/Input to the PATH of the converted files.
         */
        private void MigrateOldTests(ChallengeTest test, ExtendedChallengeModel challengeModel, Folder inputFolder)
        {
            test.InputFiles.ForEach(x => x.Name = x.Name.Replace("/", Folder.pathSeperator).Replace("\\\\", Folder.pathSeperator));
            var anotherList = test.InputFiles.Select(x => x).ToList();
            var multipleUsedFileNames = new List<string>();
            foreach (var file in test.InputFiles)
            {
                if (file.Name.Contains(Folder.pathSeperator))
                {
                    CreateNeededFoldersForOld(file, anotherList);
                }

                var isUsedByMultipleTests = int.TryParse(file.OriginalName.Substring(0, 1), out var start) && start != test.Id;
                if (isUsedByMultipleTests)
                {
                    multipleUsedFileNames.Add(file.OriginalName);
                }
            }

            anotherList = ConvertToFolderStructure(anotherList, 1);
            foreach (var file in anotherList)
            {
                if (file is Folder folder)
                {
                    inputFolder.NewFilesInFolder.Add(folder);
                    CheckIfMultipleUsedFilesOrFolder(folder, multipleUsedFileNames, test.Id.ToString());
                }
                else
                {
                    inputFolder.FilesInFolder.Add(file);
                }
            }

            CheckIfMultipleUsedFilesOrFolder(inputFolder, multipleUsedFileNames, test.Id.ToString());
            CheckForDeeperFiles(anotherList, challengeModel, test.Id.ToString());
        }

        /**
         * Checks if a file is a folder or a multiple used file, in which case there is a need to create a new instance on server side.
         * If so, it moves them to the new files, signaling it has to be created or copied.
         */
        private void CheckIfMultipleUsedFilesOrFolder(Folder parent, List<string> multipleUsed, string testId)
        {
            var FutureFilesInFolder = parent.FilesInFolder.Select(x => x).ToList();
            foreach (var file in parent.FilesInFolder)
            {
                if (multipleUsed.Any(x =>
                {
                    var splitted = x.Split(Folder.pathSeperator.ToCharArray());
                    //The first two elements of the splitted array are TestIndex and "Input", so they + 2 folder seperators need to be substracted of the name for check.
                    var name = x.Substring(splitted[0].Length + splitted[1].Length + 2);
                    return name.Equals(file.Path + file.Name);
                }))
                {
                    FutureFilesInFolder.Remove(file);
                    var newFile = new DetailedInputFile(file)
                    {
                        Content = Encoding.UTF8.GetBytes("Copy"), Path = testId + Folder.pathSeperator + "Input" + Folder.pathSeperator + file.Path
                    };
                    parent.NewFilesInFolder.Add(newFile);
                }

                if (file is Folder folder)
                {
                    FutureFilesInFolder.Remove(file);
                    parent.NewFilesInFolder.Add(folder);
                    file.Path = testId + Folder.pathSeperator + "Input" + Folder.pathSeperator + file.Path;
                    CheckIfMultipleUsedFilesOrFolder(folder, multipleUsed, testId);
                }
            }

            parent.FilesInFolder = FutureFilesInFolder;
        }

        //Adds {testId}/Input to the path of all files in Input.FilesInFolder (not Input.NewFilesInFolder!) and 
        //renames them in ChallengeModel, so they are updated on the server. NewFilesInFolder are maintained in
        //"CheckIfMultipleUsedFilesOrFolder"
        private void CheckForDeeperFiles(List<File> filesInFolder, ExtendedChallengeModel challengeModel, string id)
        {
            foreach (var file in filesInFolder)
            {
                if (file is Folder folder)
                {
                    CheckForDeeperFiles(folder.FilesInFolder, challengeModel, id);
                }

                file.Path = id + Folder.pathSeperator + "Input" + Folder.pathSeperator + file.Path;
                challengeModel.Files.Where(x => x.Name.Equals(file.Name)).ToList().ForEach(x => x.Name = file.Path + x.Name);
            }
        }

        /**
         * Due old tests fake the folder structure by renaming the files,
         * the corresponding folders containing the files need to be created.
         * This method creates all folders with corresponding name, that are contained in the files name WITHOUT PATH (name containing full path)
         * and adds them to a list, that holds all files, if they do not already exist.
         */
        public void CreateNeededFoldersForOld(File file, List<File> listToBeMappedTo)
        {
            var files = file.Name.Split(Folder.pathSeperator.ToCharArray());
            for (var i = 0; i < files.Length - 1; i++)
            {
                var folder = new File();
                var name = string.Empty;
                for (var o = 0; o <= i; o++)
                {
                    name += files[o] + Folder.pathSeperator;
                }

                folder.Name = name;
                folder.OriginalName = name;
                if (!listToBeMappedTo.Any(x => x.Name.Equals(folder.Name)))
                {
                    listToBeMappedTo.Add(folder);
                }
            }
        }

        public bool FolderHolder(string fullName, string Id)
        {
            return fullName.Equals(Id + Folder.pathSeperator + "Input" + Folder.pathSeperator) || fullName.Equals(Id + Folder.pathSeperator) ||
                   fullName.Equals(Id + Folder.pathSeperator + "Output" + Folder.pathSeperator);
        }

        //TODOS: Implementing type extraction of files when fetching them from server.
        //       Fixing folders floating right instead of left.
        //       Enabling using enter key for submitting folder updates without saving.
        //       Working on css for file edit properties.
        //       Enable folder deletion. -> Check, if it is looking good.
        //       Last modified needs to be also updated in updateAllChanges. -> Does it even matter? I think it´s forgotten everytime.
        //       Adding functionality to save buttons in edit.
    }
}
