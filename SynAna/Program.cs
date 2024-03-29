﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SynAna;
using SynAna.LexAna;

var fileName = Debugger.IsAttached ? "testfile.c" : AskUserFileName();

using var input = new StreamReader(fileName);

var folderName = Debugger.IsAttached ? "results" : AskUserFolderName();

if (!Directory.Exists(folderName))
    Directory.CreateDirectory(folderName);

var lexicalAnalyser = new Lexical(input, folderName);

var lexicalResult = lexicalAnalyser.Analyze();

var syntacticAnalyser = new Syntactic(lexicalResult);

syntacticAnalyser.Analyze();

lexicalAnalyser.Close();

string AskUserFileName() =>
    AskUser("Digite o caminho para o arquivo de entrada: ");

string AskUserFolderName() =>
    AskUser("Digite a pasta para salvar os arquivos de resultados: ");

string AskUser(string message)
{
    Console.WriteLine(message);

    return Console.ReadLine();
}