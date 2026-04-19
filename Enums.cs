public enum ItsmStatus
{
    Unknown = 0,    //mapper não conseguiu identificar
    New = 1,        //acabado de criar
    Open = 2,       //aberto
    Triaged = 3,    //avaliado e priorizado
    InProgress = 4, // alguém está a trabalhar nele
    Pending = 5,    // à espera de resposta do cliente
    OnHold = 6,     // pausado
    Resolved = 7,   // resolvido
    Closed = 8,     // completamente fechado
    Cancelled = 9,
    Reopened = 10
}

public enum ItsmTicketType
{
    Unknown = 0,
    Incident = 1,
    Task = 2,
    Subtask = 3,
    ServiceRequest = 4,
    Problem = 5,
    Change = 6,
    Case = 7,
    Other = 8
}

public enum PriorityLevel
{
    Unknown = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum ItsmResolution
{
    None = 0,              // sem resolução
    Unknown = 1,           // resolução não reconhecida
    Done = 2,
    Resolved = 3,
    Fixed = 4,
    Workaround = 5,
    Duplicate = 6,
    WontFix = 7,
    NotReproducible = 8, 
    Cancelled = 9,
    Rejected = 10
}

public enum DomainKey
{
    Unknown = 0,
    OperationalSecurity = 1,          //10%
    ThreatLandscape = 2,              // 18%
    DetectionAndResponse = 3,         // 18%
    HumanRisk = 4,                    // 10%
    VulnerabilityAndAttackSurface = 5, // 18%
    IdentityAndAccessSecurity = 6,    // 14%
    GovernanceAndResilience = 7       // 12%
}


// de onde veio o ficheiro JSON
public enum SourceSystem
{
    Jira = 1,
    Other = 2
}

// ferramenta que fez o scan de vulnerabilidades
public enum ScanEngine
{
    Unknown = 0,    //não confirmada**************************************
    Nmap = 1,
    Nessus = 2,
    Qualys = 3,
    Rapid7 = 4,
    OpenVas = 5,
    CheckPoint = 6
}

public enum VulnSeverity
{
    Unknown = 0,
    Info = 1,
    Low = 2,
    Medium = 3,
    High = 4,
    Critical = 5
}
