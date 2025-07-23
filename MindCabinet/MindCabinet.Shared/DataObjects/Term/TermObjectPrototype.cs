using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects;


public class TermObjectPrototype {
    public long? Id { get; private set; } = null;

    [JsonIgnore]
	public bool IsAssignedId { get; private set; } = false;

	public string? Term { get; set; }

	public TermObjectPrototype? Context { get; set; }

    public TermObjectPrototype? Alias { get; set; }



    public override string ToString() {
		return this.Context is not null
			? $"{this.Term ?? "-"} ({this.Context.Term ?? "-"})"
			: this.Term ?? "-";
    }
}
