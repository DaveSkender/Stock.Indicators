import setuptools

with open("README.md", "r", encoding="utf-8") as fh:
    long_description = fh.read()

setuptools.setup(
    name="skender-stock-indicators",
    version="0.0.1",
    author="Dave Skender",
    maintainer="Dong-Geon Lee",
    description="Stock indicators.  Send in historical price quotes and get back desired technical indicators such as Stochastic RSI, Average True Range, Parabolic SAR, etc.  Nothing more.",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://daveskender.github.io/Stock.Indicators/wraps/python",
    project_urls={
        "Bug Tracker": "https://github.com/DaveSkender/Stock.Indicators/issues",
        "Documentation": "https://daveskender.github.io/Stock.Indicators/wraps/python",
        "Source Code": "https://github.com/DaveSkender/Stock.Indicators/tree/master/wraps/python",
    },
    license="Apache 2.0",
    classifiers=[
        "Programming Language :: Python",
        "Programming Language :: Python :: 3.8",
        "Programming Language :: Python :: 3.9",
        "License :: OSI Approved :: Apache Software License",
        "Operating System :: OS Independent",
    ],
    platforms=["Windows", "Linux"],
    package_dir={"": "."},
    packages=setuptools.find_packages(exclude=('tests', 'tests.*')),
    package_data={
        "SkenderStockIndicators._cslib": ["lib/*.dll"],
    },
    python_requires=">=3.8",
    install_requires=[
    'pythonnet',
    ],
)
