namespace KarnelTravelGuide.Models;

public record NavLink(string Text, string Url);
public record NavItem(string Label, string Url, NavLink[] Links);
