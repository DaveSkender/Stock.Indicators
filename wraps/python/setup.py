import setuptools

with open("README.md", "r", encoding="utf-8") as fh:
    long_description = fh.read()

setuptools.setup(
    name="Here-is-package-name", # Replace with your own username
    version="0.0.1",
    author="Here is Author",
    author_email="Here-is-author@example.com",
    maintainer="Here is maintainer",
    maintainer_email="here-is-maintainer@python.org",
    description="Here is description",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/pypa/Here-is-package-name",
    project_urls={
        "Bug Tracker": "https://github.com/",
        "Documentation": "https://github.com/",
        "Source Code": "https://github.com/",
    },
    license='BSD', # BSD,MIT....
    classifiers=[
        "Programming Language :: Python :: 3",
        "License :: OSI Approved :: MIT License",
        "Operating System :: OS Independent",
    ],
    platforms=["Windows", "Linux"], # "Solaris", "Mac OS-X", "Unix"
    package_dir={"": "SkenderStockIndicators"},
    packages=setuptools.find_packages(where="SkenderStockIndicators"),
    python_requires=">=3.8",
)