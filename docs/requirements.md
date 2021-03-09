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
- Teachers should monitor the progress of their learners (including duplicate checks)
- Creators should define challenges and screen submissions for them
- Reviewers should evaluate submissions for their programming language expertise, but restricted to those challenges, they have solved by their own

Non-Functional Requirements
---------------------

> Please restrict yourself to non-functional requirements which cover the aspects of the quality goals of the ISO 25010.

- Security
  - Users must be authenticated to use the platform.
  - Compilation and execution of the source code must be performed in a sandboxed en environment.
  - Authorization must be realized by whitelisting.
- Portability
  - The challenge platform must be build and run via docker.

<!-- styling section -->
<style>
    body {text-align: justify}
</style>
