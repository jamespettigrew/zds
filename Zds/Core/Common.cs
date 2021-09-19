using System.Collections.Generic;

namespace Zds.Core
{
    public record Position(int Line, int LinePosition);
    public record ObjectRecord(Position Position, List<PathValue> PathValues);
    public record PathValue(string Path, string Value);
}