<!-- ABOUT THE PROJECT -->

## About The Project

Here is an application using the EntityFramework. This program is a parser of data from a CSV file and saves this data to a database. There is also a filtering option. The user sets the parameters through a configuration JSON file and then receives the books that match his parameters in the form of a CSV file with a unique name, and the order of the columns in this file will fully match the format in the input file. The application also supports arbitrary column order and much more. Just try it.

<!-- GETTING STARTED -->

<br><br>

## Installation

_Follow these steps to get the program set up and running correctly._
<br>

1. Clone the repo

```sh

git clone https://git.foxminded.ua/foxstudent105510/task-5-books.git

```

2. Go to the catalog with the main project

```sh

cd task-5-books/BookManager

```

3. Compile and run the project

```sh

dotnet run

```


<!-- ROADMAP -->

<br><br>

## Roadmap

-	[x] Create a console application

-	[x] Add entities

-	[x] Add data context

-	[x] Add migration to create a database

-	[x] Add the ability to enter a file path through the console

-	[x] Write parsing of data from CSV file to database

-	[x] Write ignoring duplicates

-	[x] Create a filter class

-	[x] Implement parsing of the configuration JSON file into the created class

-	[x] Display the names of filtered books in the console

-	[x] Output detailed information to a CSV file

-	[x] Implement the same column order in the input and output files

-	[x] Make output file names unique

-	[x] Create Unit tests
