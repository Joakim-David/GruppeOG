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
Our application utilizes an automated CI/CD pipeline to continuously harden the system and
ensure vulnerabilities are not introduced during development.

The pipeline  four CodeQL, our SAST tool, which notified the developers about a vulnerability to Cross-Site Scripting (XSS) attacks 
caused by unsanitized user input in Request.Query["search"]. This vulnerability was patched by applying System.Net.WebUtility.HtmlEncode() 
to ensure the input is treated as plain text rather than exceutable code. 






## Availability and Scaling
<!-- Author(s): Bondo -->
TODO: How do you handle availability and scaling in your systems?

Our application runs on a single DigitalOcean droplet using Docker Swarm, which orchestrates two replicas of our Chirp container, each capped at 200 MB of memory. Swarm's built-in load balancer distributes incoming requests evenly across both replicas, and all services are configured with restart_policy: on-failure, so the swarm manager automatically restarts any failed container. A DigitalOcean reserved IP is assigned to the droplet via Terraform, ensuring a stable endpoint even if the underlying VM is replaced.

We have applied both vertical and horizontal scaling. We vertically scaled our managed PostgreSQL cluster by upgrading from 1 GB to 2 GB of RAM, which improved query performance. We horizontally scaled the application layer by moving from a single container to two Swarm replicas. The entire infrastructure — droplet, database cluster, firewall rules, and reserved IP is defined in Terraform and can be reproduced from scratch with a single terraform apply.

The main limitation is single points of failure: we rely on one droplet and one database node. Adding a database replica would be the highest-value improvement, both for data redundancy and because the database is the primary performance bottleneck.


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