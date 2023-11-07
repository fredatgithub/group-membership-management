// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { IStrings } from '../../../IStrings';

export const strings: IStrings = {
  emptyList: 'There are no GMM managed groups',
  loading: 'Loading',
  addOwner204Message: 'Added Successfully.',
  addOwner400Message: 'GMM is already added as an owner.',
  addOwner403Message: 'You do not have permission to complete this operation.',
  addOwnerErrorMessage:
    'We are having trouble adding GMM as the owner. Please try again later.',
  bannerMessageStart: "Need help? ",
  clickHere: "Click here",
  bannerMessageEnd: " to learn more about how Membership Management works in your organization.",
  okButton: 'OK',
  groupIdHeader: 'Enter Group ID',
  groupIdPlaceHolder: 'Group ID',
  addOwnerButton: 'Add GMM as an owner',
  membershipManagement: 'Membership Management',
  learnMembershipManagement:'Learn how Membership Management works in your organization',
  AdminConfig: {
      labels: {
          pageTitle: "Admin Configuration",
          hyperlinks: "Hyperlinks",
          description: "Provide hyperlinks to the following organization specific information so that your users are empowered to leverage XMM to its fullest."
      },
      hyperlinkContainer: {
          address: "Address",
          addHyperlink: "Add hyperlink",
          dashboardTitle: "Dashboard",
          dashboardDescription: "This is the link that shows on the top right corner of the dashboard. It takes you to an internal site that has all the details on how to leverage XMM at your organization. This could include FAQs, contact information, SLAs, etc.",
          invalidUrl: "Invalid URL"
      }
  },
  Authentication: {
    loginFailed: 'An unexpected error occurred during login.'
  },
  JobDetails: {
    labels: {
      pageTitle: 'Membership Details',
      sectionTitle: 'Membership Details',
      lastModifiedby: 'Last Modified by',
      groupLinks: 'Group Links',
      destination: 'Destination',
      type: 'Type',
      name: 'Name',
      ID: 'ID',
      configuration: 'Configuration',
      startDate: 'Start Date',
      endDate: 'End Date',
      lastRun: 'Last Run',
      nextRun: 'Next Run',
      frequency: 'Frequency',
      frequencyDescription: 'Every {0} hrs',
      requestor: 'Requestor',
      increaseThreshold: 'Increase Threshold',
      decreaseThreshold: 'Decrease Threshold',
      thresholdViolations: 'Threshold Violations',
      sourceParts: 'Source Parts',
      membershipStatus: 'Membership status',
      sync: 'Sync',
      enabled: 'Enabled',
      disabled: 'Disabled',
    },
    descriptions: {
      lastModifiedby: 'User who made the last change to this job.',
      startDate: 'Date of the onboarding of this job into GMM.',
      endDate: 'Date of the last run of this job.',
      type: 'Sync type.',
      id: 'Object ID of the destination group.',
      lastRun: 'Time of the last run of this job.',
      nextRun: 'Next run time for this job.',
      frequency: 'How often this job runs.',
      requestor: 'User who requested this job to be onboarded into GMM.',
      increaseThreshold:
        'Number of users that can be added from the target group expressed as a percentage of the current size of the group.',
      decreaseThreshold:
        'Number of users that can be removed from the target group expressed as a percentage of the current size of the group.',
      thresholdViolations: 'Number of times a threshold was exceeded.',
    },
    MessageBar: {
      dismissButtonAriaLabel: 'Close',
    },
    Errors:{
      jobInProgress: 'Job is in progress. Please try again later.',
      notGroupOwner: 'You are not an owner of this group.',
      internalError: 'We can\'t process your request at this time. Please try again later',
    },
    openInAzure: 'Open in Azure',
    viewDetails: 'View Details',
    editButton: 'Edit',
  },
  JobsList: {
    listOfMemberships: 'List of memberships',
    ShimmeredDetailsList: {
      toggleSelection: 'Toggle selection',
      toggleAllSelection: 'Toggle selection for all items',
      selectRow: 'select row',
      ariaLabelForShimmer: 'Content is being fetched',
      ariaLabelForGrid: 'Item details',
      columnNames: {
        name: 'Name',
        type: 'Type',
        lastRun: 'Last Run',
        nextRun: 'Next Run',
        status: 'Status',
        actionRequired: 'Action Required',
      },
    },
    MessageBar: {
      dismissButtonAriaLabel: 'Close',
    },
    PagingBar: {
      previousPage: 'Prev',
      nextPage: 'Next',
      page: 'Page',
      of: 'of',
      display: 'Display',
      items: 'Items per page',
    },
    JobsListFilter: {
      filters: {
        ID: {
          label: 'ID',
          placeholder: 'Search',
          validationErrorMessage: 'Invalid GUID!',
        },
        status: {
          label: 'Status',
          options: {
            all: 'All',
            enabled: 'Enabled',
            disabled: 'Disabled',
          },
        },
        actionRequired: {
          label: 'Action Required',
          options: {
            all: 'All',
            thresholdExceeded: 'Threshold Exceeded',
            customerPaused: 'Customer Paused',
            membershipDataNotFound: 'No users in the source',
            destinationGroupNotFound: 'Destination Group Not Found',
            notOwnerOfDestinationGroup: 'Not Owner Of Destination Group',
            securityGroupNotFound: 'Security Group Not Found',
          },
        },
      },
      filterButtonText: 'Filter',
      clearButtonTooltip: 'Clear Filters',
    },
    NoResults: 'No memberships found.',
  },
  ManageMembership: {
    manageMembershipButton: 'Manage Membership',
    labels: {
      abandonOnboarding: 'Abandon Onboarding?',
      abandonOnboardingDescription: 'Are you sure you want to abandon the in-progress onboarding and go back?',
      confirmAbandon: 'Yes, go back',
      pageTitle: 'Manage Membership',
      step1title: 'Step 1: Select Destination',
      step1description: 'Please select the destination type and the destination whose membership you want to manage.',
      selectDestinationType: 'Select Destination Type',
      selectDestinationTypePlaceholder: 'Select an option',
      searchDestination: 'Search destination',
      searchDestinationPlaceholder: 'Search for a group',
      noResultsFound: 'No results found',
      appsUsed: 'This group uses the following apps:',
      outlookWarning: 'There are important settings that should be considered before sending email to this Outlook group. Follow the instructions on your organization.',
      ownershipWarning: 'Warning: GMM is not the owner of this group! It will not be able to manage membership for this group until you add it.',
      step2title: 'Step 2: Membership Configuration',
      step2description: 'Define the source membership for the destination.',
      advancedQuery: 'Advanced Query',
      advancedView: 'Advanced View',
      query: 'Query',
      validQuery: 'Query is valid.',
      invalidQuery: 'Failed to parse query. Ensure it is valid JSON.',
      validateQuery: 'Validate Query',
    }
  },
  needHelp: 'Need help?',
  next: 'Next',
  close: 'Close',
  cancel: 'Cancel',
  learnMore: 'Learn more',
  errorItemNotFound: 'Item not found',
  welcome: 'Welcome',
  back: 'Back',
  backToDashboard: 'Back to dashboard',
  version: 'Version',
};