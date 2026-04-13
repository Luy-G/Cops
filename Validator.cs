public static class ItsmDtoValidator
{
    public static bool IsValid(JsonSogilubDto? dto)
    {
        if (dto == null)
            return false;

        if (string.IsNullOrWhiteSpace(dto.TicketKey) &&
            string.IsNullOrWhiteSpace(dto.IssueId))
            return false;

        if (dto.CreatedAt == null)
            return false;

        return true;
    }
}