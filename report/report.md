---
title: "ITU-MiniTwit Report"
subtitle: "\\centerline{BSc Group G}\\centerline{DevOps, Software Evolution and Software Maintenance}\\centerline{BSDSESM1KU}"
author: "\\shortstack{Emilie Bliddal Ravn Larsen (emrl@itu.dk) \\\\\\\\ Jacob Folkmann (jafo@itu.dk) \\\\\\\\ Jacob Hørberg (jacho@itu.dk) \\\\\\\\ Joakim-David (jpre@itu.dk) \\\\\\\\ Rasmus Bondo (rabh@itu.dk)}"
date: "Spring 2026"
geometry: margin=2.5cm
fontsize: 11pt
numbersections: true
toc: true
header-includes:
  - \usepackage{float}
  - \floatplacement{figure}{H}
---

# System's Perspective
## Design and Architecture
<!-- Author(s): Bondo -->
TODO: Design and architecture of your ITU-MiniTwit systems.

## Dependencies
<!-- Author(s): Jacob eller Jacob -->
TODO: All dependencies of your ITU-MiniTwit systems on all levels of abstraction and development stages. That is, list and briefly describe all technologies and tools you applied and depend on.

## Current State of the System
<!-- Author(s): Emilie -->
TODO: Describe the current state of your systems, for example using results of static analysis and quality assessments.

# Process' Perspective
TOBEDELETED: This perspective should clarify how code or other artifacts come from idea into the running system and everything that happens on the way.


## CI/CD Chains
<!-- Author(s): Emilie -->
TODO: A complete description and illustration of stages and tools included in the CI/CD pipelines, including deployment and release of your systems.

## Monitoring
<!-- Author(s): Jacob -->
TODO: How do you monitor your systems and what precisely do you monitor?

## Logging
<!-- Author(s): Jacob -->
TODO: What do you log in your systems and how do you aggregate logs?

## Security Assessment
<!-- Author(s): Joakim -->
TODO: Brief description of how you security hardened your systems.

## Scaling and Load Balancing
<!-- Author(s): Bondo -->
TODO: How do you handle availability and scaling in your systems?


# Reflection Perspective
TOBEDELETED: Describe the biggest issues, how you solved them, and which are major lessons learned with regards to:

## Evolution and Refactoring
<!-- Author(s): Emilie og Joakim -->
TODO: Describe the biggest issues, interactions, and bugs during the project.
NOTE: convert from old chirp to new thing with API
NOTE: Migration to postgres

## operation
<!-- Author(s): Bondo -->
TODO: From docker compose to terraform and docker swarm.

## maintenance
<!-- Author(s): Jacob eller Jacob -->
TODO: 

# Use of generative AI
<!-- Author(s): Joakim  -->

TOBEDELETED: ITU's rules on the use of generative AI apply for this report too. They are described here and in detail here. Please follow them. For your report that means that you have to state which generative AI tools have been used for which task(s) in your projects. Additionally, describe how generative AI tools have been used and briefly reflect and discuss how they supported or hindered your work process.
NOTE: like 2 senteces about this

## References
<!-- Link all artifacts: repositories, issue trackers, monitoring/logging systems, etc. -->

- **Repository:** <https://github.com/Joakim-David/GruppeOG>
- **Chirp URL:** <https://chirpitu.live>
- **Monitoring:** Grafana at `http://209.38.190.12:3000`
- **Issue Tracker:** <https://github.com/Joakim-David/GruppeOG/issues>