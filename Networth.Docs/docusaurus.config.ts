import { themes as prismThemes } from "prism-react-renderer";
import type { Config } from "@docusaurus/types";
import type * as Preset from "@docusaurus/preset-classic";
import remarkMath from "remark-math";
import rehypeKatex from "rehype-katex";

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const config: Config = {
    title: "Networth Documentation",
    tagline: "Architecture and API Documentation for the Networth Application",
    favicon: "img/networth-icon.svg",

    // Future flags, see https://docusaurus.io/docs/api/docusaurus-config#future
    future: {
        v4: true, // Improve compatibility with the upcoming Docusaurus v4
    },

    stylesheets: [
        {
            href: "https://cdn.jsdelivr.net/npm/katex@0.13.24/dist/katex.min.css",
            type: "text/css",
            integrity:
                "sha384-odtC+0UGzzFL/6PNoE8rX/SPcQDXBJ+uRepguP4QkPCm2LBxH3FA3y+fKSiJ+AmM",
            crossorigin: "anonymous",
        },
    ],

    // Set the production url of your site here
    url: "https://networth.tbushell.co.uk",
    // Set the /<baseUrl>/ pathname under which your site is served
    // For GitHub pages deployment, it is often '/<projectName>/'
    baseUrl: "/",

    // GitHub pages deployment config.
    organizationName: "TomSB1423",
    projectName: "NetWorth",
    trailingSlash: false,

    onBrokenLinks: "throw",

    i18n: {
        defaultLocale: "en",
        locales: ["en"],
    },

    markdown: {
        mermaid: true,
    },
    themes: ["@docusaurus/theme-mermaid"],

    presets: [
        [
            "classic",
            {
                docs: {
                    sidebarPath: "./sidebars.ts",
                    routeBasePath: "/",
                    editUrl:
                        "https://github.com/TomSB1423/NetWorth/tree/main/Networth.Docs/",
                    remarkPlugins: [remarkMath],
                    rehypePlugins: [rehypeKatex],
                },
                blog: {
                    showReadingTime: true,
                    feedOptions: {
                        type: ["rss", "atom"],
                        xslt: true,
                    },
                    editUrl:
                        "https://github.com/TomSB1423/NetWorth/tree/main/Networth.Docs/",
                    remarkPlugins: [remarkMath],
                    rehypePlugins: [rehypeKatex],
                },
                theme: {
                    customCss: "./src/css/custom.css",
                },
                sitemap: {
                    lastmod: "date",
                    changefreq: "weekly",
                    priority: 0.5,
                    ignorePatterns: ["/tags/**"],
                    filename: "sitemap.xml",
                },
            } satisfies Preset.Options,
        ],
    ],

    plugins: [
        "docusaurus-plugin-image-zoom",
        [
            "@docusaurus/plugin-ideal-image",
            {
                quality: 70,
                max: 1030, // max resized image's size.
                min: 640, // min resized image's size. if original is lower, use that size.
                steps: 2, // the max number of images generated between min and max (inclusive)
                disableInDev: false,
            },
        ],
        [
            "@easyops-cn/docusaurus-search-local",
            {
                hashed: true,
                language: ["en"],
                indexDocs: true,
                indexBlog: false,
                indexPages: true,
            },
        ],
        [
            "@scalar/docusaurus",
            {
                label: "API Reference",
                route: "/api",
                configuration: {
                    spec: {
                        url:
                            process.env.API_SPEC_URL ||
                            "http://localhost:7071/api/swagger.json",
                    },
                    theme: "purple",
                    darkMode: true,
                },
            },
        ],
    ],

    themeConfig: {
        zoom: {
            selector: ".markdown :not(em) > img",
            config: {
                // options you can specify via https://github.com/francoischalifour/medium-zoom#usage
                background: {
                    light: "rgb(255, 255, 255)",
                    dark: "rgb(50, 50, 50)",
                },
            },
        },
        image: "img/networth-social-card.jpg",
        colorMode: {
            respectPrefersColorScheme: true,
        },
        navbar: {
            title: "Networth",
            logo: {
                alt: "Networth Logo",
                src: "img/networth-icon.svg",
                href: "/",
            },
            items: [
                {
                    type: "docSidebar",
                    sidebarId: "architectureSidebar",
                    position: "left",
                    label: "Documentation",
                },
                { to: "/blog", label: "Blog", position: "left" },
                {
                    href: "https://github.com/TomSB1423/NetWorth",
                    position: "right",
                    className: "header-github-link",
                    "aria-label": "GitHub repository",
                },
            ],
        },
        footer: {
            style: "dark",
            links: [
                {
                    title: "Documentation",
                    items: [
                        {
                            label: "User Guide",
                            to: "/user-guide/overview",
                        },
                        {
                            label: "API Reference",
                            to: "/api",
                        },
                    ],
                },
                {
                    title: "Project",
                    items: [
                        {
                            label: "GitHub Repository",
                            href: "https://github.com/TomSB1423/NetWorth",
                        },
                    ],
                },
            ],
            copyright: `Copyright © ${new Date().getFullYear()} Networth Project.`,
        },
        mermaid: {
            theme: { light: "neutral", dark: "dark" },
        },
        prism: {
            theme: prismThemes.github,
            darkTheme: prismThemes.dracula,
            additionalLanguages: ["csharp", "json", "typescript", "bash"],
        },
    } satisfies Preset.ThemeConfig,
};

export default config;
