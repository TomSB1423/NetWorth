import type {SidebarsConfig} from '@docusaurus/plugin-content-docs';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

/**
 * Creating a sidebar enables you to:
 - create an ordered group of docs
 - render a sidebar for each doc of that group
 - provide next/previous navigation

 The sidebars can be generated from the filesystem, or explicitly defined here.

 Create as many sidebars as you want.
 */
const sidebars: SidebarsConfig = {
  architectureSidebar: [
    {
      type: 'category',
      label: 'Getting Started',
      items: ['intro', 'architecture'],
    },
    {
      type: 'category',
      label: 'Architecture',
      items: [
        'architecture/overview',
        'architecture/aspire',
        'architecture/backend',
        'architecture/frontend',
        'architecture/data-flow',
      ],
    },
    {
      type: 'category',
      label: 'Components',
      items: [
        'components/functions',
        'components/application',
        'components/infrastructure',
        'components/domain',
      ],
    },
  ],
};

export default sidebars;
