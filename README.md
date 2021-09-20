## Quick Start

1. [Install the .NET 5 SDK appropriate for your system.](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Ensure you are using a [supported terminal emulator](#terminal-emulator-support).
2. Clone repository.

**HTTPS**
```bash
git clone https://github.com/jamespettigrew/zds.git
```

**SSH**
```bash
git clone git@github.com:jamespettigrew/zds.git
```

3. Change to project directory.
```bash
cd zds
```

4. Run tests to make sure everything is working.
```bash
dotnet test
```

5. Run
```bash
dotnet run --project Zds --data Samples/Data --relations Samples/relations.json
```

## Usage
```bash
./Zds --help
Zds 1.0.0
Copyright (C) 2021 Zds

  --data         Path to search for data. Defaults to current directory.

  --relations    Path to file containing relations.

  --help         Display this help screen.

  --version      Display version information.
  ```

The application can be exited via `Ctrl-c`.

Upon starting, the provided source and [relations](#relations) files will be indexed. You will then be prompted to build a query, providing source file, path and search value.

**Constraints**:
- Search values are exact matched.
- Nested objects can be searched, and arrays can be searched, but objects within arrays cannot be searched.
- Leave the search value blank if you wish to search for objects that have no value for the specified path.

Any matching objects will be returned in pages:
- To view the next page, press the right arrow key.
- To view the previous page, press the left arrow key.
- Press any other key to start a new search.

Demo
[![asciicast](https://asciinema.org/a/gCdjPm1PuyVgKB4KEdcTubM05.svg)](https://asciinema.org/a/gCdjPm1PuyVgKB4KEdcTubM05)

### Relations
Relations can be defined between objects. Each matching search result will also be accompanied by any related results per the defined relations.

e.g. A relation between tickets and users can be defined on `ticket.assignee_id = user._id`. Any search results for tickets will have associated results for the assigned user.

Relations should be defined as a JSON array adhering to the following schema:
```json
[
   {
      "from": {
         "source": "string",
         "path": "string"
      },
      "to": {
         "source": "string",
         "path": "string"
      }
   }
]
```

`source` being the filename of the particular data file, and `path` being the JSON path of the property.

A sample relations definition has been included in the Samples directory.
```json
[{
  "from": {
    "source": "users.json",
    "path": "_id"
  },
  "to": {
    "source": "tickets.json",
    "path": "assignee_id"
  }
}]
```

**Note** that relations are bidirectional.

## Design
I envisioned a tool that would facilitate exploration of semi-structured JSON data.

My assumptions:
- The user may not have a strong idea of the shape of the data.
- The user may want to perform multiple queries within a session.
- The user may not be a developer / may be only semi-skilled.
- The data may not be entirely uniform in structure.
- The kind of data may vary per user i.e. it won't always be `tickets.json` and `users.json`.
- The user is an English speaker and the application does not require internationalisation.


With these assumptions in mind, I had the following design goals:
- **Optimise for multiple searches**
  - The data is streamed into an index up front, trading off start-up time for improved query latency.
- **Hint about the shape of the data**
  - As part of the indexing process, the application discovers the underlying schema(s).
  - At query time, the user is supplied with the discovered files and schemas to choose query paths from.
- **Keep queries simple**
  - The user is guided through query building by selection prompts.
  - Results are paginated and don't require knowledge of further CLI tools for interpretation/analysis.
- **Handle data files that are non-uniform in structure.**
  - Values are indexed against the source file and JSON path.
  - Two different objects within the same file need not have the same shape.
- **Allow arbitrary relations**
  - Relations can be defined between any `source:(file, path)` and `destination:(file, path)`
- **Minimise memory usage**
  - The index stores positions within files rather than entire objects. Upon querying, matches are returned as positions and the object is retrieved from disk.
    - **Note**: While I think this concept is sound, [there are some implementation details that threw a spanner in the works.](#fetching-objects-from-disk)
  - While data may fit in memory for now, it may not in future.
    - Should this tool end up operating within a larger scale cloud environment it will be much cheaper if we can keep as much on disk as possible.
  - Just because we can use as much memory as an Electron app doesn't mean we should strive to.
  
The design consists of fairly loosely-coupled core components with a CLI porcelain over the top. It's highly flexible, only enforcing a fairly minimal set of rules over the shape of data it can query. It's also very extensible; it could fairly easily be adapted to read files from S3, or some other form of seekable stream.
  
In practice, .NET would not be my first choice for building a CLI tool. It requires a fat runtime to be deployed alongside the application code and can suffer from JIT cold-starts.
But for the purposes of this particular task it's the platform that best demonstrates my professional competency.

## Limitations

### Fetching objects from disk
The strategy to achieve low latency search with minimal memory consumption was to index only positions of objects in files, retrieving the object from disk before displaying results.

There is a flaw in the current implementation in that the JSON library used ([JSON.NET](https://github.com/JamesNK/Newtonsoft.Json)) parses files through the use of a character stream, not a byte stream. The position info for JSON tokens is consequently represented as a line and character pair. Retrieving the object from disk at a later time based upon this line and character pair requires seeking through the file until the appropriate number of lines and characters have been enumerated. For objects not near the start of the file, this is unnecessarily heavy on IO and results in significantly greater search latency.

I investigated alternative JSON libraries such as Microsoft's newer [System.Text.Json](https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/). This library uses a byte stream underneath, meaning it should be possible to store the byte offsets of objects and retrieve them from disk with very little IO and latency. However, interacting with this library at the low level needed to accomplish this is tricky and requires working with some fairly esoteric language features.

I think the original concept would be achievable with additional work, but I'm leaving it as out of scope for now.

### Data structures
For the sake of simplicity, I elected to use an inverted index (in the form of a dictionary) for matching search terms against objects.
While simple and providing *O(1)* lookup performance, it would not be capable of handling more complex queries. If in future there was a need for additional operators, such equality or range queries, a data stucture such as a sorted array (or BTree if dynamic insertions are required) would be warranted.

### Terminal Emulator Support
The .NET ecosystem has not placed as much emphasis in the past on CLI type tools as other ecosystems. As such, there's limited choice of libraries for working with terminal emulators.

[The library I used for CLI interaction](https://github.com/spectreconsole/spectre.console) requires terminal emulators that support ANSI escape sequences.

#### Known Supported
**Linux**
- XTerm

**Mac**
  - Terminal.app
  - iTerm

**Windows**
- Powershell
- Windows Terminal

#### Known Unsupported
- Alacritty

### Searching arrays
Only arrays of primitive types are supported for querying. Objects within arrays are ignored.

## Future Work
- [Address flaws in fetching objects from disk.](#fetching-objects-from-disk)
- If the intended user is more competent than my assumptions, perhaps support more complex queries by integrating a parser combinator library such as [Superpower](https://github.com/datalust/superpower).
- Rethink how pagination should work with large numbers of related results.
- Allow redefining relations after application start
- Exporting of search results
