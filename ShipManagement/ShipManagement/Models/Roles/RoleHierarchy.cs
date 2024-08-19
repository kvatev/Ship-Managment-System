namespace ShipManagement.Models.Roles;

public class RoleHierarchy
{
    public static bool CanAssignTaskTo(string assignerRole, string assigneeRole)
    {
        var roleHierarchy = new Dictionary<string, int>
        {
            {"Адмирал,", 1}, 
            {"Вицеадмирал,", 2}, 
            {"Контраадмирал,", 3}, 
            {"Флотилен адмирал,", 4}, 
            {"Капитан ранг I,", 5}, 
            {"Капитан ранг II,", 6}, 
            {"Капитан ранг III,", 7},
            {"Капитан-лейтенант,", 8}, 
            {"Старши лейтенант,", 9}, 
            {"Лейтенант,", 10}, 
            {"Офицерски кандидат,", 11}, 
            {"Мичман,", 12}, 
            {"Главен старшина,", 13}, 
            {"Старшина I степен,", 14}, 
            {"Старшина II степен,", 15}, 
            {"Старши матрос,", 16}, 
            {"Матрос", 17}
        };

        if (roleHierarchy.TryGetValue(assignerRole, out var assignerLevel) &&
            roleHierarchy.TryGetValue(assigneeRole, out var assigneeLevel))
        {
            return assignerLevel < assigneeLevel;
        }

        return false;
    }
}
