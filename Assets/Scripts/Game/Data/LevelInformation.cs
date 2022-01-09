using System;

public class LevelInformation : SceneIndependent {
	public Level select { get; private set; }

	public LevelInformation(Level select) {
		this.select = select;
	}
}