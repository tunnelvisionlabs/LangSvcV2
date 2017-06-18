# The "LangSvcV2" Project

[![Build status](https://ci.appveyor.com/api/projects/status/7xibxi9sa92q7gb2/branch/master?svg=true)](https://ci.appveyor.com/project/sharwell/langsvcv2/branch/master)

[![Join the chat at https://gitter.im/tunnelvisionlabs/LangSvcV2](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/tunnelvisionlabs/LangSvcV2?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Welcome to the "Language Service V2" project, or "LangSvcV2" for short. Originally called "SimpleC", this
code represents a rewrite of the frameworks, tools, and language services targeting Visual Studio 2012+
and based on experiences with prior extensions. At this point, the repository contains the code for many
different Visual Studio extensions with varying degrees of dependence on other projects within the
solution.

The long-term plan involves separating the repository into separate repositories, each of which provides
a specific set of features in a reusable distribution with clearly defined dependencies. However, Tunnel
Vision Laboratories has released the reference code in this repository for two purposes.

1. **Immediate Availability**: Since it is not clear when the refactoring operation will be complete, it
   is not clear when the individual repositories for each component will be available. This repository
   provides the ability to reference the source code immediately.
2. **Preservation of History**: Separating this repository will be greatly simplified if preservation of
   the source control history within each project is not crucial. With this repository available online,
   history for critical components (e.g. the syntax highlighting framework) are guaranteed to be
   preserved.
