# Relatório Técnico - Sala de Segurança VR

## 1. Identificação

- **Projeto:** Sala de Segurança VR
- **Autor:** Paulo Victor da Costa Vilarins
- **Curso/atividade:** Web 3.0 - Residência em TIC 29
- **Limitação de hardware relatada:** o desenvolvimento foi priorizado no desktop, pois o notebook disponível poderia apresentar limitações de desempenho para o Unity Editor e para pacotes XR.
- **Repositório:** <https://github.com/duckrwx/cftv-metaverso>

## 2. Objetivo

O objetivo da atividade foi criar um primeiro ambiente de Realidade Virtual em Unity, com cena organizada, elementos 3D básicos e possibilidade de navegação no editor. O projeto foi mantido propositalmente simples para priorizar uma entrega funcional, compreensível e compatível com execução local.

## 3. Conceito do ambiente

A cena representa uma sala de segurança com elementos associados a monitoramento: mesa, monitor, cofre, câmera CFTV decorativa, porta, paredes, teto, iluminação e skybox. A proposta visual conversa com o tema de CFTV sem implementar captura real de vídeo, gateway, blockchain ou processamento externo, pois esses pontos ficariam fora do escopo imediato da atividade.

## 4. Ferramentas e versão

- **Motor:** Unity 6000.3.19f1.
- **Renderização:** Universal Render Pipeline (URP) 17.3.0.
- **Entrada:** Unity Input System.
- **Controle de versão:** Git e GitHub.
- **Execução prevista:** Unity Editor em desktop.

## 5. Configuração XR e Meta Quest

O projeto foi estruturado para ser uma primeira base de ambiente VR. Nesta versão, a navegação principal foi implementada para PC, com teclado e mouse, permitindo testar a cena diretamente no Unity Editor sem headset.

No estado atual do repositório, a cena, a navegação no PC e os módulos básicos de XR do Unity estão presentes. A configuração completa do Meta XR SDK e do XR Plugin Management para Android/Meta Quest deve ser finalizada no Unity Editor antes da entrega, caso o avaliador exija essa validação como requisito obrigatório.

Para uma evolução com Meta Quest, o caminho técnico recomendado é:

1. Instalar ou validar o Meta XR SDK no Package Manager.
2. Ativar XR Plugin Management nas configurações do projeto.
3. Selecionar Android como plataforma de build.
4. Configurar Oculus/Meta como provider XR.
5. Substituir ou complementar o `Player_XR` atual por um XR Origin compatível.
6. Fazer build para o dispositivo Meta Quest após validar cena, câmera e controles.

Essa separação reduz risco na entrega atual: primeiro a cena abre e funciona no editor; depois ela pode ser adaptada para headset.

## 6. Cena principal

**Arquivo da cena:** `Assets/Scenes/SalaSegurancaVR.unity`

**Registro visual da cena:** `docs/imagens/ambiente-unity.png`

Elementos principais:

- Chão.
- Quatro paredes.
- Teto.
- Mesa de monitoramento.
- Monitor.
- Cofre.
- Câmera CFTV decorativa.
- Teclado.
- Mouse.
- Cadeira.
- Porta.
- Luz direcional.
- Luz de teto.
- Câmera principal.
- Controlador de navegação no PC.

## 7. Organização da hierarquia

A cena foi organizada com grupos nomeados em português:

```text
--- GERENCIAMENTO ---
  SistemaDeEventos

--- PLAYER ---
  Player_XR
    Camera_Principal

--- AMBIENTE ---
  Chao
  Parede_Norte
  Parede_Sul
  Parede_Leste
  Parede_Oeste
  Teto

--- OBJETOS DA SALA DE SEGURANCA ---
  Mesa
  Monitor
  Cofre
  Camera_CFTV
  Teclado
  Mouse
  Cadeira
  Porta

--- ILUMINACAO ---
  Luz_Direcional
  Luz_Teto
```

## 8. Assets e elementos usados

| Elemento | Tipo | Origem | Função |
| --- | --- | --- | --- |
| Chão | Primitivo Unity | Unity | Base de navegação da sala |
| Paredes e teto | Primitivos Unity | Unity | Delimitação do ambiente |
| Mesa | Composição de cubos | Unity | Mesa de monitoramento |
| Monitor | Cubo e cilindro | Unity | Representar supervisão de CFTV |
| Cofre | Cubos e cilindro | Unity | Elemento central de segurança |
| Câmera CFTV | Cubos e cilindro | Unity | Referência visual ao tema do projeto |
| Teclado | Cubo | Unity | Detalhe manual da estação de monitoramento |
| Mouse | Primitivo Unity | Unity | Detalhe manual da estação de monitoramento |
| Cadeira | Composição de cubos | Unity | Complementar a estação de trabalho |
| Porta | Cubo | Unity | Elemento ambiental da sala |
| Luzes | Light components | Unity | Iluminação da cena |
| Skybox | Material procedural | Unity/URP | Composição visual do ambiente |

## 9. Navegação e interação

O projeto possui o script `Assets/Scripts/PCPlayerController.cs`, que permite testar o ambiente no editor:

- `W`, `A`, `S`, `D`: movimentação horizontal.
- Mouse: controle de câmera.
- `Espaço`: subir.
- `Ctrl`: descer.
- `Shift`: acelerar.
- `Esc`: travar ou liberar cursor.

Essa solução atende ao teste local e evita depender de headset para validar a cena.

## 10. Geração da cena

O projeto também inclui o script editor `Assets/Editor/SalaSegurancaBuilder.cs`. Ele cria automaticamente a cena `SalaSegurancaVR`, materiais, hierarquia, objetos principais, iluminação e controlador de navegação.

No Unity Editor, a reconstrução pode ser feita pelo menu:

```text
Sala Seguranca VR -> Construir Cena
```

## 11. Estrutura versionada no GitHub

Foram versionados os arquivos necessários para abrir o projeto Unity:

```text
Assets/
Packages/
ProjectSettings/
docs/
README.md
.gitignore
```

Arquivos gerados localmente pelo Unity, como `Library/`, `Temp/`, `Logs/`, `Obj/`, `Build/` e `UserSettings/`, foram excluídos pelo `.gitignore`.

## 12. Como executar

1. Clonar o repositório:

```bash
git clone https://github.com/duckrwx/cftv-metaverso.git
```

2. Abrir a pasta no Unity Hub.
3. Usar a versão Unity 6000.3.19f1 ou uma versão compatível.
4. Abrir a cena:

```text
Assets/Scenes/SalaSegurancaVR.unity
```

5. Pressionar Play no Unity Editor.
6. Navegar usando teclado e mouse.

## 13. Limitações

- A versão atual não implementa CFTV real, streaming de vídeo, gateway, blockchain ou assinatura de segmentos.
- A câmera CFTV é decorativa e serve para caracterizar visualmente a sala.
- A execução foi priorizada no Unity Editor, sem obrigatoriedade de headset.
- A adaptação para Meta Quest exige validação adicional de XR Plugin Management, Android Build Support e Meta XR SDK no Unity Editor.

## 14. Conclusão

O projeto entrega uma cena VR inicial, navegável e organizada em Unity. A escolha por uma sala de segurança simples permitiu atender ao escopo da atividade com baixo risco técnico, mantendo uma base coerente para evolução posterior com Meta Quest, XR Origin e recursos mais avançados de interação.
