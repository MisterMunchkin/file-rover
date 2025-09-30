# File Rover

File Rover is an agentic file renaming system powered by Semantic Kernel and MetadataExtractor. It enables intelligent, metadata-driven file renaming using plugins and AI orchestration.

---

## Architecture Overview

### 1. **Agentic Service Layer**

-   **FileRenamerAgenticService**:  
    Orchestrates AI-driven file renaming.
    -   Uses Semantic Kernel's `ChatCompletionAgent` for LLM interaction.
    -   Handles user requests, prepares conventions and metadata fields, and triggers plugin execution.
    -   Maintains chat history via `ChatHistoryAgentThread`.

### 2. **Plugin Layer**

-   **FileRenamerPlugin**:  
    Implements the actual file renaming logic.
    -   Exposes functions (e.g., `rename_image`) for the agent to invoke.
    -   Uses metadata conventions and field maps to generate new file names.

### 3. **Metadata Extraction Layer**

-   **FileRenamerMetadataExtractorImageService**:  
    Extracts metadata from various image formats using MetadataExtractor.
    -   Supports multiple formats (JPEG, PNG, GIF, BMP, GeoTIFF).
    -   Provides a unified method (`ExtractMetadata`) to retrieve file and image metadata.
    -   Handles directory type mapping and tag extraction robustly.

### 4. **Business Objects (Concept)**

-   **Business Objects**:  
    (Planned) Encapsulate both attributes and business logic.
    -   Each object manages its own state and logic.
    -   Business logic is only applied to the object's own attributes, not to others.
    -   Promotes encapsulation and separation of concerns.

---

## Data Flow

1. **User Request**:  
   User provides a file path and renaming convention.

2. **Agent Orchestration**:  
   The agent receives the request, determines the appropriate plugin function, and invokes it.

3. **Metadata Extraction**:  
   The plugin calls the metadata extraction service to gather required metadata fields.

4. **File Renaming**:  
   The plugin applies the naming convention using extracted metadata and renames the file.

5. **Result Return**:  
   The agent returns the result or error to the user.

---

## Notable Patterns & Potential Antipatterns

-   **Function-Calling Orchestration**:  
    Correctly uses Semantic Kernel's orchestration to invoke plugins, not just output function calls as JSON.

-   **Explicit Directory Type Mapping**:  
    Uses a dictionary of supported directory types for robust metadata extraction.

-   **Business Logic Encapsulation**:  
    The planned "business objects" concept is a good pattern for encapsulation.  
    **Note:** Ensure business objects do not become "anemic" (i.e., only data containers without logic).

-   **Prompt Instructions**:  
    Instructions to the agent are explicit, reducing ambiguity and improving reliability.

-   **Potential Antipatterns**:
    -   **Manual JSON Serialization for Requests**:  
        Consider using structured objects or SK's native function-calling features for better type safety.
    -   **Async Without Await**:  
        Ensure async methods perform asynchronous operations or use `Task.FromResult` for clarity.
    -   **Direct Plugin Invocation Logic in Agent**:  
        If logic for plugin selection grows, consider refactoring to a strategy or factory pattern.

---

## Extending the System

-   **Add More Plugins**:  
    Implement additional plugins for other file types or business operations.
-   **Enhance Business Objects**:  
    Move more logic into business objects for better encapsulation.
-   **Improve Error Handling**:  
    Standardize error responses and logging across layers.

---

## Dependencies

-   [.NET 9.0](https://dotnet.microsoft.com/)
-   [Semantic Kernel](https://github.com/microsoft/semantic-kernel)
-   [MetadataExtractor](https://github.com/drewnoakes/metadata-extractor-dotnet)

---

## How to Use

1. Configure conventions and metadata fields.
2. Run the agentic service and provide a file path.
3. The agent will rename the file based on metadata and conventions.

---

## Feedback

If you spot any architectural antipatterns or have suggestions, please open an issue or contribute!
