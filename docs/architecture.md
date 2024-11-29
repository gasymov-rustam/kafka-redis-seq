graph TD
    classDef service fill:#f9f,stroke:#333,stroke-width:2px
    classDef interface fill:#ff9,stroke:#333,stroke-width:2px
    classDef external fill:#9ff,stroke:#333,stroke-width:2px
    classDef component fill:#fff,stroke:#333,stroke-width:2px
    classDef mediator fill:#ffd,stroke:#333,stroke-width:2px
    classDef command fill:#ffe,stroke:#333,stroke-width:2px

    %% API Layer
    subgraph "API Layer"
        API[KafkaRedis.Api]
    end

    %% Domain Layer
    subgraph "Domain Layer"
        subgraph "Interfaces"
            IKP[IKafkaProducer]
            IKC[IKafkaConsumer]
            IRS[IRedisService]
            IMS[IMessageSend]
            ICF[IKafkaConsumerFactory]
            ILS[ILoggingService]
        end

        subgraph "Models"
            KS[KafkaSettings]
            RS_SET[RedisSettings]
            SS[SeqSettings]
        end

        subgraph "Commands"
            PMC[ProduceMessageCommand]
            CMC[ConsumeMessageCommand]
        end
    end

    %% Infrastructure Layer
    subgraph "Infrastructure Layer"
        subgraph "Services"
            KP[KafkaProducerService]
            KC[KafkaConsumerService]
            RS[RedisService]
            MS[MessageSend]
            CF[KafkaConsumerFactory]
            Timer[MessageTimerService]
            LS[LoggingService]
        end

        subgraph "Handlers"
            PMH[ProduceMessageHandler]
            CMH[ConsumeMessageHandler]
        end

        MediatR[MediatR]
    end

    %% External Services
    subgraph "External Services"
        Kafka[Apache Kafka]
        Redis[(Redis)]
        KafkaUI[Kafka UI]
        RedisCommander[Redis Commander]
        Seq[Seq Server]
    end

    %% Service Implementation
    KP -.implements.-> IKP
    KC -.implements.-> IKC
    RS -.implements.-> IRS
    MS -.implements.-> IMS
    CF -.implements.-> ICF
    LS -.implements.-> ILS

    %% Service Dependencies
    KP -.uses.-> KS
    KC -.uses.-> KS
    RS -.uses.-> RS_SET
    KC -.uses.-> CF
    LS -.uses.-> SS

    %% Message Flow
    Timer --triggers--> MS
    MS --sends--> MediatR
    MediatR --handles--> PMH
    PMH --uses--> KP
    PMH --uses--> RS
    PMH --sends--> CMC
    
    %% External Connections
    KP --produces--> Kafka
    KC --consumes--> Kafka
    RS --caches--> Redis
    LS --logs--> Seq

    %% API Dependencies
    API --> MediatR
    
    %% Mediator Pattern
    PMH -.handles.-> PMC
    CMH -.handles.-> CMC

    %% Style Application
    class KP,KC,RS,MS,CF,Timer,LS service
    class IKP,IKC,IRS,IMS,ICF,ILS interface
    class Kafka,Redis,KafkaUI,RedisCommander,Seq external
    class API component
    class MediatR mediator
    class PMC,CMC command

## Legend

- Blue - External Services (Kafka, Redis)
- Pink - Services (KafkaProducer, RedisService, etc.)
- Yellow - Interfaces (IKafkaProducer, IRedisService, etc.)
- White - API Layer
- Brown - Mediator
- Light Yellow - Commands

## Architecture Layers

### API Layer
- Application entry point
- Minimal logic, only request routing through MediatR

### Domain Layer
- Service interfaces
- Configuration models
- MediatR commands
- No business logic implementation

### Infrastructure Layer
- Service implementations
- Command handlers
- External service integration
- Background tasks (MessageTimerService)

### Key Components

#### MessageTimerService
- Background service
- Periodically sends test messages
- Uses MessageSend for delivery

#### MessageSend
- Message sending abstraction
- Uses MediatR for command dispatch

#### ProduceMessageHandler
- Handles ProduceMessageCommand
- Sends message to Kafka
- Stores message in Redis
- Initiates message consumption

#### KafkaConsumerFactory
- Creates Kafka consumers
- Manages consumer lifecycle

## Logging Infrastructure

The application uses Serilog for structured logging with Seq integration. The logging infrastructure is configured in the following components:

### Components

#### LoggingService
- Located in `KafkaRedis.Infrastructure.Services`
- Implements `ILoggingService` interface
- Configures global Serilog logger with Seq integration
- Reads configuration from appsettings.json
- Outputs logs to both Console and Seq server

#### LoggingExtensions
- Located in `KafkaRedis.Infrastructure.Extensions`
- Provides extension methods for DI setup and middleware configuration
- `AddLoggingServices`: Configures logging services in DI container
- `UseLoggingMiddleware`: Sets up Serilog request logging with enriched context

#### SeqSettings
- Located in `KafkaRedis.Domain.Models`
- Contains Seq server configuration
- Configured via appsettings.json

### Configuration
Logging is configured through appsettings.json and includes:
- Seq server URL
- Log levels
- Console output
- Request logging enrichment

## Design Patterns
- CQRS (via MediatR)
- Mediator (MediatR for command handling)
- Factory (KafkaConsumerFactory)
- Repository (RedisService)
- Background Service (MessageTimerService)
