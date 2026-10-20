# Project Apollo Risk Register

## Risk 1: Production hardware delay

Severity: High
Probability: High

Northwind Components has reported a six-week delay for the production hardware
batch. The production cutover depends on this hardware.

Mitigation:

- Ask Northwind for a partial shipment.
- Evaluate whether the test hardware can be temporarily promoted.
- Prepare a revised cutover plan.

## Risk 2: Cutover support unavailable

Severity: Medium
Probability: Medium

The current service agreement requires ten business days of notice for weekend
support. A late schedule change may leave the project without support during the
new cutover window.

Mitigation:

- Ask Contoso to reserve a backup weekend.
- Escalate revised support requirements before the next steering committee.

## Risk 3: Retention rule sign-off delayed

Severity: Medium
Probability: Medium

Business owners have not completed final sign-off for retention rules. Migration
cannot be approved for production without this sign-off.

Mitigation:

- Schedule a focused review with business owners.
- Prepare a fallback rule set for low-risk content classes.

## Risk 4: Contingency drawdown above threshold

Severity: Medium
Probability: Medium

The six-week supplier delay is expected to increase project run costs and may
push contingency drawdown above the internal budget threshold for Project Apollo.

Mitigation:

- Reforecast the cutover budget with the revised hardware date.
- Ask the steering committee to approve additional contingency if the partial
  shipment option is not available.
