ITD-Challenge Platform - Requirements
========================

> This collection of requirements is not complete as many features were developed via an internal ticket system in the past.
> However, please extend the requirements for new features to gain completeness over time.
> Please verify, that there are also automated tests to support the correctness of the implementation.

Functional Requirements
---------------------

> Document the functional requirements in the format: “\<the profiteer><should/must/can>\<do something>”
> Use “should” as default: Should is mandatory as long as there are no good reasons against implementation.

- Learners should enroll themselves in groups
- Learners should view challenges (including description, example program output, difficulty level, ...)
- Learners should use a programming language of their choose to solve the challenge
- Learners should receive feedback on whether their submission passes all automated tests, or if not, the reason for error
- Participants should receive a review of their submission to learn best programming practices from colleagues
- Teachers should administrate learning groups (define challenges to support the learning)
- Teachers should administrate their learners (reset passwords and challenges, mark learning group as passed)
- Teachers should monitor the progress of their learners (including points, duplicate checks, is group passed)
- Creators should define challenges and screen submissions for them
- Reviewers should evaluate submissions for their programming language expertise, but restricted to those challenges, they have solved by their own

Non-Functional Requirements (ISO 25010)
---------------------

- Security
  - The challenge-platform should be extensible to use different authentication providers. At least LDAP and Shibboleth should be supported. Authentication should be done with ASP.NET Core if none of those providers is available.
  - There are views and endpoints which should be restricted by authenticated roles. On top, there are entities like the submitted source code which require access control.
  - Compilation and execution e.g. interpretation of third-party source code must be performed in a sandboxed environment.
- Portability
  - The challenge platform should be portable to different server hardware architectures like ARMv8 and AMD64.
  - The challenge platform should be deployed with minimal host system dependencies.
- Performance
  - The initial web page load time as well as the navigation between pages should not take longer than 2 seconds.
- Integrity
  - The maintenance should not lead to lost or inconsistent data (submission, rankings, ...).
  - The system should enable backups.

<!-- styling section -->
<style>
    body {text-align: justify}
</style>
