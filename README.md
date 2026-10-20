# Simple Microsoft Agent Framework (MAF) and Alfresco Sample Repository

This repository contains the sample application for the article [How to Connect Microsoft Agent Framework to Alfresco in C# RAG Without a Vector Database](https://dev.to/stefankopfxaldon/how-to-connect-microsoft-agent-framework-to-alfresco-in-c-rag-without-a-vector-database-56e4).

The example shows a simple way to use Alfresco Content Services from a C#
Microsoft Agent Framework application through OpenDMA:

```text
MAF ChatClientAgent
  -> on-demand TextSearchProvider tool
  -> agent-generated focused search terms
  -> C# search callback
  -> OpenDMA REST API
  -> Alfresco
```

The example is intentionally small. It is meant to show the integration surface,
not a production RAG architecture.

## Prerequisites

- .NET 9 SDK
- an existing Alfresco deployment
- an OpenDMA REST endpoint for Alfresco
- an OpenAI API key

With the defaults, the model provider is OpenAI, so set `OPENAI_API_KEY`.

## OpenDMA Endpoint For Alfresco

The application needs an OpenDMA REST endpoint for Alfresco. The quickest way to
map Alfresco to the OpenDMA data model and expose such an endpoint is to run the
[ECI Server](https://github.com/xaldon/eci-server). It is available free of
charge for non-production use.

Start it with Docker Compose:

```bash
git clone https://github.com/xaldon/eci-server.git
cd eci-server/docker_compose
docker compose up -d
```

After the service is running:

1. Open the web UI at `http://localhost:7070`.
2. Initialize the admin account.
3. Accept the license agreement.
4. Install a free-of-charge license key.
5. Navigate to "Admin" > "Connections".
6. Add a new connection to `Alfresco Content Services`.
7. Choose "Automatically detect parameters with Smart Setup" for your existing Alfresco Content Services target server.
   If Alfresco is running in Docker as well, use `host.docker.internal`.
8. Save the new connection as `Alfresco @ <hostname>`.
9. Navigate to "Admin" > "REST Endpoints".
10. Add a new REST Endpoint.
11. Set the slug to `opendma/alf`.
12. Select the `Alfresco @ <hostname>` connection.
13. Keep the proposed "Inbound Authentication" settings: "HTTP Basic" and "Propagate Inbound".
14. Save the new REST Endpoint.

The sample expects the OpenDMA endpoint at:

```text
http://localhost:7070/opendma/alf
```

To verify this REST endpoint, open `http://localhost:7070/opendma/alf/` with a
trailing slash and authenticate with your Alfresco credentials.

This README assumes you already have Alfresco running. It does not include
Alfresco installation instructions.

## Sample Data

The sample uses markdown files to keep the focus on the MAF/OpenDMA retrieval
path. It does not demonstrate PDF, DOCX, XLSX, OCR, or document conversion.

The `sample-data` folder is copied verbatim from the sibling CrewAI article
sample. Use it when you want the shared Project Apollo data set.

The `sample-data-dapted` folder starts from the same files and adds the small
extra details needed by the questions used in this MAF sample, such as milestone
penalties and contingency drawdown.

Create an Alfresco site:

```text
Title: Project Apollo
Short name: project-apollo
```

Upload the markdown files from either `sample-data` or `sample-data-dapted` into
the site's Document Library:

```text
Project Apollo
  Contracts/
    Supplier Agreement.md
    Service Agreement.md
  Reports/
    Status Report September.md
    Status Report October.md
  Risks/
    Risk Register.md
```

For the sample questions below, use `sample-data-dapted`.

## Configuration

Set the environment variables:

```text
OPENDMA_ENDPOINT=http://localhost:7070/opendma/alf
OPENDMA_USERNAME=admin
OPENDMA_PASSWORD=admin
OPENDMA_REPOSITORY_ID=Alfresco
ALFRESCO_SITE=project-apollo
ALFRESCO_SHARE_URL=http://localhost:8080/share
OPENAI_MODEL=gpt-4o-mini
OPENAI_API_KEY=...
```

Optional settings:

```text
MAX_SEARCH_RESULTS=8
MAX_EXCERPT_LENGTH=1500
```

`ALFRESCO_SHARE_URL` is only used to create best-effort source links in the
returned `TextSearchProvider` results. If your Alfresco Share URL differs,
adjust it.

## Build

```bash
dotnet build OpenDma.Demo.Alfresco.Maf.Simple.sln
```

## Run

```bash
dotnet run --project src/OpenDma.Demo.Alfresco.Maf.Simple
```

The application starts an interactive console:

```text
OpenDMA Alfresco MAF demo
Type a question, or 'exit' to quit.

Question:
```

For repository questions, the agent creates one or more focused searches. The
console shows the terms, generated AFTS query, and number of documents returned:

```text
Question: What does Alfresco say about supplier delays and budget risk in Project Apollo?

> Search terms: supplier delay
> AFTS query: TEXT:("supplier" AND "delay") AND SITE:"project-apollo"
> Found 4 documents.

> Search terms: contingency budget
> AFTS query: TEXT:("contingency" AND "budget") AND SITE:"project-apollo"
> Found 1 documents.

The Project Apollo documents identify...
```

The exact search terms and number of tool calls are model-generated and can vary.
On-demand retrieval also adds a model/tool round trip compared with searching the
raw question before model invocation.

Try:

```text
What does Alfresco say about supplier delays and budget risk in Project Apollo?
```

```text
Which document mentions milestone penalties?
```

```text
What changed between the September and October status reports?
```

```text
What does the risk register say about contingency drawdown?
```

Use `exit` or `quit` to end the application.

## What The Sample Does

For every question, the application:

1. Exposes `TextSearchProvider` to the agent as the `SearchAlfresco` function tool.
2. Lets the agent create focused search terms and make separate searches for distinct topics.
3. Validates the terms and builds a conservative, explicitly AND-connected AFTS query.
4. Restricts every query to the configured Alfresco site.
5. Searches Alfresco through OpenDMA.
6. Reads markdown/plain-text content from matching documents.
7. Returns short `TextSearchProvider.TextSearchResult` values with source names and links.
8. Lets the agent answer from the retrieved context and cite its sources.

The model controls when and what to search for, so its generated terms are treated
as untrusted input. The application accepts only a small number of ordinary terms,
adds the site restriction itself, and never accepts raw AFTS from the model.

## What The Sample Does Not Do

- No vector database.
- No embeddings.
- No repository synchronization process.
- No PDF/DOCX/XLSX text extraction.
- No MEDI integration.
- No explicit ECM tools for browsing sites or metadata.
- No production authentication design.
- No UI beyond a console chat loop.

## Troubleshooting

If no documents are found, verify:

- the OpenDMA endpoint opens at `http://localhost:7070/opendma/alf/`;
- you can authenticate with the configured Alfresco credentials;
- the `Project Apollo` site short name is `project-apollo`;
- markdown files were uploaded into the site Document Library;
- Alfresco has indexed the uploaded content.

If the application fails before the first question, verify:

- `OPENAI_API_KEY` is set;
- the .NET 9 SDK is installed;
- NuGet packages have restored successfully.
